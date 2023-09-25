# Migration Guide
The following compares the .NET [Laserfiche.Repository.Api.Client](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client) NuGet package with the [Laserfiche.Repository.Api.Client.V2](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client.V2) NuGet package at time of initial release.

The `Laserfiche.Repository.Api.Client` accesses the v1 Laserfiche Repository APIs and the `Laserfiche.Repository.Api.Client.V2` accesses the v2 Laserfiche Repository APIs. Additionally, many API function signatures have been updated in the v2 client. See the charts below for the functions in the v1 client that correspond to the functions in v2.

See [here](https://api.laserfiche.com/repository/v2/changelog#2023-10) for more details on the changes between the v1 and v2 Laserfiche Repository APIs.

### Attributes
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetTrusteeAttributeKeyValuePairsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#a8c758bdcb9ab4fe48ae68ee79c392d83)         | ListAttributesAsync                 |
| [GetTrusteeAttributeKeyValuePairsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#aca24527d1034d9317bd3c8960468bdde)  | ListAttributesForEachAsync          |
| [GetTrusteeAttributeKeyValuePairsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#a329751535c109dba25994ec2e0b8a282) | ListAttributesNextLinkAsync         |
| [GetTrusteeAttributeValueByKeyAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#ae83b062e920497c2fabfc0cde46adb6d)            | GetAttributeAsync                   |

### Audit Reason
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetAuditReasonsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_audit_reasons_client.html#a149c7dd595603974e748c41b86efd7b1)         | ListAuditReasonsAsync                 |

