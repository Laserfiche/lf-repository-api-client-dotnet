#!/usr/bin/env python3
"""
Post-nswag patch: rewrite nswag's hard-coded null-throw checks for multipart
form-data parameters that the swagger marks as optional.

nswag always emits

    if (x == null)
        throw new ArgumentNullException("parameters.X");
    else
    {
        <append to MultipartFormDataContent>
    }

for every multipart property, ignoring the swagger's `required` list. For
parameters the server accepts as absent (e.g. imageFiles, optional request
payloads), this turns a legitimate omission into a client-side crash. The
script rewrites each such block into

    if (x != null)
    {
        <append to MultipartFormDataContent>
    }

for parameters whose operationId's multipart schema does NOT list them as
required. Required parameters are left alone so that callers still see the
throw when they forget a mandatory field.

Mirrors the JS-side helper in lf-api-js@dbe809c. Idempotent. Reads swagger.json
next to this file and rewrites ../src/Clients/RepositoryClients.cs in place.
"""
import json
import re
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
SWAGGER = HERE / "swagger.json"
CLIENT_CS = HERE.parent / "src" / "Clients" / "RepositoryClients.cs"


def load_optional_multipart_params(swagger_path):
    with swagger_path.open() as f:
        spec_doc = json.load(f)
    out = {}
    for path_item in spec_doc.get("paths", {}).values():
        for verb_spec in path_item.values():
            if not isinstance(verb_spec, dict):
                continue
            op_id = verb_spec.get("operationId")
            if not op_id:
                continue
            rb = verb_spec.get("requestBody", {})
            for ct, media in rb.get("content", {}).items():
                if "multipart" not in ct:
                    continue
                schema = media.get("schema", {})
                required = set(schema.get("required", []))
                props = set(schema.get("properties", {}).keys())
                optional = props - required
                if optional:
                    out[op_id] = optional
    return out


# Match nswag's null-throw block on a single multipart param:
#     if (NAME == null)
#         throw new ArgumentNullException("parameters.PASCAL");
#     else
#
# Followed by either a single statement or a "{ ... }" block. Capture enough
# of the surrounding context to reason about the following body (which we keep
# verbatim). The block signature is stable across nswag 14.x C# output.
NULL_CHECK = re.compile(
    r"^(?P<indent>[ \t]+)if \((?P<name>[a-z]\w*) == null\)\n"
    r"[ \t]+throw new ArgumentNullException\(\"parameters\.(?P<pascal>[A-Z]\w*)\"\);\n"
    r"(?P<indent2>[ \t]+)else\n",
    re.MULTILINE,
)

# Match nswag-generated C# method signatures so we can attribute each null-check
# to its enclosing method. Example:
#     public virtual async System.Threading.Tasks.Task<Entry> ImportEntryAsync(
# The char class includes `,` and space so the return type can contain nested
# generics like Task<IDictionary<string, IList<string>>>. Newlines are not
# included, so the return-type capture stays on a single line.
METHOD_SIG = re.compile(
    r"^\s+public virtual async [\w\.<>, ]+ (?P<name>[A-Z]\w+)Async\(",
    re.MULTILINE,
)


def method_name_to_op_id(method_name, op_ids):
    """C# method name `<OpId>Async` maps 1:1 to operationId."""
    for op_id in op_ids:
        if op_id == method_name:
            return op_id
    return None


def pascal_to_camel(pascal):
    """`ImageFiles` -> `imageFiles` (swagger property casing)."""
    return pascal[0].lower() + pascal[1:] if pascal else pascal


def enclosing_method(src, pos):
    last = None
    for m in METHOD_SIG.finditer(src):
        if m.start() > pos:
            break
        last = m
    return last.group("name") if last else None


def patch(src, optional_map):
    op_ids = list(optional_map.keys())
    patched = 0

    def replace(match):
        nonlocal patched
        var_name = match.group("name")
        pascal = match.group("pascal")
        method = enclosing_method(src, match.start())
        if method is None:
            return match.group(0)
        op_id = method_name_to_op_id(method, op_ids)
        if op_id is None:
            return match.group(0)
        camel = pascal_to_camel(pascal)
        if camel not in optional_map.get(op_id, set()):
            return match.group(0)
        indent = match.group("indent")
        patched += 1
        # Rewrite to `if (x != null)` and drop the `else` keyword — the body
        # block (or statement) that follows the original `else` is left as-is.
        return f"{indent}if ({var_name} != null)\n"

    return NULL_CHECK.sub(replace, src), patched


def main():
    if not SWAGGER.exists():
        print(f"[patch_optional_multipart] {SWAGGER} not found; skipping", file=sys.stderr)
        return 0
    if not CLIENT_CS.exists():
        print(f"[patch_optional_multipart] {CLIENT_CS} not found; skipping", file=sys.stderr)
        return 0

    optional_map = load_optional_multipart_params(SWAGGER)
    if not optional_map:
        print("[patch_optional_multipart] no optional multipart params in swagger")
        return 0

    src = CLIENT_CS.read_text(encoding="utf-8")
    new_src, patched = patch(src, optional_map)
    if patched == 0:
        print("[patch_optional_multipart] no null-check blocks rewritten (already patched or nothing to patch)")
    else:
        CLIENT_CS.write_text(new_src, encoding="utf-8")
        ops = ", ".join(sorted(optional_map))
        print(f"[patch_optional_multipart] rewrote {patched} null-check(s) across operations: {ops}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
