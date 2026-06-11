import argparse
import io
import json
import os
import requests
import sys

def download_file_to_json(url):
    '''
    Downloads a file from the specified url and returns it as json.
    '''
    response = requests.get(url)
    if response.status_code != requests.codes.ok:
        raise Exception(f'Error while downloading file from {url}. status code {response.status_code}')
    else:
        return response.json()

def replace_with_override_data(json_data, json_override_data):
    '''
    Replaces elements in json_data with elements in json_override_data.
    If an element in json_override_data has value None, then it will instead be deleted.
    '''
    for key in json_override_data:
        value = json_override_data[key]
        if (value == None):
            del json_data[key]
        else:
            json_data[key] = value
    return json_data

def sort_status_codes(json_data):
    '''
    Sorts the status codes for each api route.
    '''
    for path_element in json_data['paths'].values():
        for api_element in path_element.values():
            if 'responses' in api_element:
                api_element['responses'] = dict(sorted(api_element['responses'].items(), key=lambda item: item[0]))
    return json_data

def normalize_servers(json_data, canonical_url):
    '''
    Forces the OpenAPI servers[] to the canonical (production) URL. A swagger document downloaded
    from a locally-running server carries that server's host (e.g. http://localhost:11211/repository),
    which NSwag otherwise bakes into the generated client's baseUrl default. Normalizing here makes the
    production URL the unconditional default regardless of the regen source; pass an empty canonical_url
    to keep the source servers as-is (the only way to intentionally bake a localhost default).
    '''
    if canonical_url:
        json_data['servers'] = [{'url': canonical_url}]
    return json_data


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Downloads a swagger file')
    parser.add_argument('--swagger-url',
                        help='The url to download the swagger file.',
                        default='https://api.laserfiche.com/repository/swagger/v2/swagger.json')
    parser.add_argument('--swagger-override-filepath',
                        help='Optional json file with override values to replace in the swagger file.')
    parser.add_argument('--output-filepath',
                        help='Downloads the swagger file to this filepath.')
    parser.add_argument('--canonical-server-url',
                        default='https://api.laserfiche.com/repository',
                        help='Force the OpenAPI servers[] to this URL so a local/dev swagger source '
                             'never bakes its host into the generated client baseUrl default. '
                             'Pass an empty string to keep the source servers as-is.')
    args = parser.parse_args()

    if args.swagger_url is None:
        raise Exception('Must specify the "swagger-url".')
    if args.output_filepath is None:
        raise Exception('Must specify the "output-filepath".')
    

    swagger_json = download_file_to_json(args.swagger_url)
    swagger_json = sort_status_codes(swagger_json)
    swagger_json = normalize_servers(swagger_json, args.canonical_server_url)

    if args.swagger_override_filepath is not None:
        with open(args.swagger_override_filepath, 'r') as swagger_override_file:
            swagger_override_json = json.load(swagger_override_file)
        swagger_json['components']['schemas'] = replace_with_override_data(swagger_json['components']['schemas'], swagger_override_json['components']['schemas'])

    with open(args.output_filepath, 'w') as json_file:
        json.dump(swagger_json, json_file, indent=2)
