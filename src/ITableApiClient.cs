// Copyright (c) Laserfiche.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Net.Http.Headers;

namespace Laserfiche.Table.Api.Client
{
    public interface ITableApiClient
    {
        HttpRequestHeaders DefaultRequestHeaders { get; }
        ITableClient TableClient { get; }
    }
}