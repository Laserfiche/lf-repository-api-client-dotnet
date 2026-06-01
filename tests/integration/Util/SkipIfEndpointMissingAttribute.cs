// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Laserfiche.Repository.Api.Client.IntegrationTest.Util
{
    /// <summary>
    /// Marks a test (class or method) as conditional on a set of OpenAPI <c>operationId</c>s being
    /// present in the swagger document of the API server under test (the BaseUrl from
    /// <c>APISERVER_REPOSITORY_API_BASE_URL</c>). When any named operation is missing the test is
    /// reported as Inconclusive instead of Failed, with a message naming the missing operation(s)
    /// and the BaseUrl that was probed.
    ///
    /// Replaces the <c>[Ignore("Temporarily ignored: cloud test server not yet updated...")]</c>
    /// pattern. The skip self-clears as soon as the deployed server's swagger contains the
    /// operation — no follow-up PR needed.
    ///
    /// See <c>site-api-repository/docs/design-server-client-preview-nuget-workflow.md</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SkipIfEndpointMissingAttribute : System.Attribute
    {
        public string[] OperationIds { get; }

        public SkipIfEndpointMissingAttribute(params string[] operationIds)
        {
            OperationIds = operationIds ?? Array.Empty<string>();
        }
    }
}
