# Migration Guide
The following guide compares the .NET [Laserfiche.Repository.Api.Client](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client) NuGet package with the [Laserfiche.Repository.Api.Client.V2](https://www.nuget.org/packages/Laserfiche.Repository.Api.Client.V2) NuGet package at time of initial release.

The `Laserfiche.Repository.Api.Client` accesses the v1 Laserfiche Repository APIs and the `Laserfiche.Repository.Api.Client.V2` accesses the v2 Laserfiche Repository APIs. Many API function signatures have been updated in the v2 client. See the tables below for the functions in the v1 client that correspond to the functions in the v2 client.

See [here](https://api.laserfiche.com/repository/v2/changelog#2023-10) for more details on the changes between the v1 and v2 Laserfiche Repository APIs.

### Attributes
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetTrusteeAttributeKeyValuePairsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#a8c758bdcb9ab4fe48ae68ee79c392d83) | ListAttributesAsync |
| [GetTrusteeAttributeKeyValuePairsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#aca24527d1034d9317bd3c8960468bdde) | ListAttributesForEachAsync |
| [GetTrusteeAttributeKeyValuePairsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#a329751535c109dba25994ec2e0b8a282) | ListAttributesNextLinkAsync |
| [GetTrusteeAttributeValueByKeyAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_attributes_client.html#ae83b062e920497c2fabfc0cde46adb6d) | GetAttributeAsync |

### Audit Reason
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetAuditReasonsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_audit_reasons_client.html#a149c7dd595603974e748c41b86efd7b1) | ListAuditReasonsAsync |

### Entries
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [AssignEntryLinksAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ad770937542b9736c23cd58a4fbcf70d4) | [SetLinksAsync]() |
| [AssignFieldValuesAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#aa1a031de833647ebd25ef34db55f56a5) | [SetFieldsAsync]() |
| [AssignTagsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a7cef12f4851dc0ba5ee22f1f86525ccc) | [SetTagsAsync]() |
| [CopyEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ac00a117d43169e8a3971afbc83f7de6f) | [StartCopyEntryAsync]() |
| [CreateOrCopyEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a7d4d97d4b300acbbd42bb551c174a71e) | Functionality split into [CreateEntryAsync]() and [CopyEntryAsync]() |
| [DeleteAssignedTemplateAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a1b85fa31c541ad2fbd0b9cfc30c64a5d) | [RemoveTemplateAsync]() |
| [DeleteDocumentAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a4ad5f40793ce7b2e213fd1a748b16409) | [DeleteElectronicDocumentAsync]() |
| [DeleteEntryInfoAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ae96b3e54604730ea3277866279f7eb6e) | [StartDeleteEntryAsync]() |
| [DeletePagesAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ab1349d3aa159378000cb437acf0f97ce) | [DeletePagesAsync]() |
| [ExportDocumentAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a1470dfd90371fb323988105369a8f6bc) | [ExportEntryAsync]() |
| [ExportDocumentWithAuditReasonAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a978411d253a22d0e4d25814f932c8d58) | [ExportEntryAsync]() |
| [GetDocumentContentTypeAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ac8cbc2f12beedeaebf9f1ad138a78ede) | Removed |
| [GetDynamicFieldValuesAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a907dbce6ff4825fffe741ddc4cf643b8) | [ListDynamicFieldValuesAsync]() |
| [GetEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a5bac96a649f58527210f1a6bdd2298d2) | [GetEntryAsync]() |
| [GetEntryByPathAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a64dc6b02934ac522d943d4411745851b) | [GetEntryByPathAsync]() |
| [GetEntryListingAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ad3516c007ef89ee2ec5f0dd683008413) | [ListEntriesAsync]() |
| [GetEntryListingForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#abb26d3014c0059b1dabf00c1e6d1f7ab) | [ListEntriesForEachAsync]() |
| [GetEntryListingNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#abd93254af7cdd55d57c1591c392dd683) | [ListEntriesNextLinkAsync]() |
| [GetFieldValuesAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#af1b63de71d635547ba548b3896e1327d) | [ListFieldsAsync]() |
| [GetFieldValuesForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#afb6ef1df5944395e3e6c1f9e2a783d96) | [ListFieldsForEachAsync]() |
| [GetFieldValuesNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ac5554c62d645a75c7bce7eefebc28770) | [ListFieldsNextLinkAsync]() |
| [GetLinkValuesFromEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#aaea36ccb6bfe5b47dde5c3c80f2718bd) | [ListLinksAsync]() |
| [GetLinkValuesFromEntryForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#aff4aa0ff11c72bbb33ad99ea2f9b2a4a) | [ListLinksForEachAsync]() |
| [GetLinkValuesFromEntryNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a88ebd615c0715b1ccb277349f44d0794) | [ListLinksNextLinkAsync]() |
| [GetTagsAssignedToEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a6665cffda9df25f0e15299a86f678132) | [ListTagsAsync]() |
| [GetTagsAssignedToEntryForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#aca2ac9d2a4cbce5147afc99a5d12d21e) | [ListTagsForEachAsync]() |
| [GetTagsAssignedToEntryNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a578305a52ebc1c2a71cf58ecb03dd5ce) | [ListTagsNextLinkAsync]() |
| [ImportDocumentAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a42a4208de033cc2ed1d3ea5a1e38dc6b) | [ImportEntryAsync]() |
| [MoveOrRenameEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#ad679c65d75b107e074b5ff6fb55583c2) | [UpdateEntryAsync]() |
| [WriteTemplateValueToEntryAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_entries_client.html#a60b37bc99ee3592619e4dfb0e387175e) | [SetTemplateAsync]() |
| -- | [CreateMultipartUploadUrlsAsync]() |
| -- | [StartImportUploadedPartsAsync]() |
| -- | [StartExportEntryAsync]() |

### FieldDefinitions
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetFieldDefinitionByIdAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_field_definitions_client.html#af54f7f6c2fc8483f23f46a0dfe4ef6fa) | [GetFieldDefinitionAsync]() |
| [GetFieldDefinitionsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_field_definitions_client.html#ac8aeec3ef689479ad6d685b6f032fcce) | [ListFieldDefinitionsAsync]() |
| [GetFieldDefinitionsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_field_definitions_client.html#a814b4a6c239aa413b99d778de43ae823) | [ListFieldDefinitionsForEachAsync]() |
| [GetFieldDefinitionsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_field_definitions_client.html#a5d401c66ed150e899482c55962808787) | [ListFieldDefinitionsNextLinkAsync]() |

### LinkDefinitions
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetLinkDefinitionByIdAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_link_definitions_client.html#ad0498cee93a5787ea1f77f123d352142) | [GetLinkDefinitionAsync]() |
| [GetLinkDefinitionsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_link_definitions_client.html#ac051692dea787b64289dfbab373c57ad) | [ListLinkDefinitionsAsync]() |
| [GetLinkDefinitionsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_link_definitions_client.html#addca64f588dc9affb94af9ef49801683) | [ListLinkDefinitionsForEachAsync]() |
| [GetLinkDefinitionsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_link_definitions_client.html#a2e31988bf6fdc13ed1da18ee969d2c8d) | [ListLinkDefinitionsNextLinkAsync]() |

### Repositories
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetRepositoryListAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_repositories_client.html#a5ecbfb1a31163c346f782234a53dfa79) | [ListRepositoriesAsync]() |
| [GetSelfHostedRepositoryListAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/class_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_repositories_client.html#ae70b4f0d7a4b99f2b218d5228004fc13) | [ListSelfHostedRepositoriesAsync]() |

### Searches
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [CancelOrCloseSearchAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#a81aceed6c81498edee5f7ab95f203a63) | [CancelTasksAsync]() |
| [CreateSearchOperationAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#a3d07f4a470ae89d142e549d68fc2e547) | [StartSearchEntryAsync]() |
| [GetSearchContextHitsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#ac7a96ee9757952029b0ed40cdafae5b4) | [ListSearchContextHitsAsync]() |
| [GetSearchContextHitsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#a9b9704c45896e1703abad6aeef6106be) | [ListSearchContextHitsForEachAsync]() |
| [GetSearchContextHitsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#aa7bd46d9f1e3afb2c181979f0204141e) | [ListSearchContextHitsNextLinkAsync]() |
| [GetSearchResultsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#ad66e1327d9a3cbb8ac32c12a5271ce6b) | [ListSearchResultsAsync]() |
| [GetSearchResultsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#a04369b3943ad2b076c4dbc4e03ccb44e) | [ListSearchResultsForEachAsync]() |
| [GetSearchResultsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#acc390ce3ecdc7cad4c81ec683d78fa13) | [ListSearchResultsNextLinkAsync]() |
| [GetSearchStatusAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_searches_client.html#aa042b5f6c372096df438c8ed5b964520) | [ListTasksAsync]() |

### ServerSession
The [IServerSessionClient](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_server_session_client.html) has been removed in `Laserfiche.Repository.Api.Client.V2`.

### SimpleSearches
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [CreateSimpleSearchOperationAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_simple_searches_client.html#a257138909be9b3f91c44ab531ab84ced) | [SearchEntryAsync]() |

### TagDefinitions
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetTagDefinitionByIdAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tag_definitions_client.html#a59f12fca5dbef371ed1264056cbfdfec) | [GetTagDefinitionAsync]() |
| [GetTagDefinitionsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tag_definitions_client.html#ad5b1afaa16c0ed67370733e465fadc6f) | [ListTagDefinitionsAsync]() |
| [GetTagDefinitionsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tag_definitions_client.html#a268cb154f40232125c508c836bef5d61) | [ListTagDefinitionsForEachAsync]() |
| [GetTagDefinitionsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tag_definitions_client.html#a5ca6e85ebb95f00bbf238136a735c295) | [ListTagDefinitionsNextLinkAsync]() |

### Tasks
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [CancelOperationAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tasks_client.html#a33b80b3c01e7ebb202dcc34be31ee1ae) | [CancelTasksAsync]() |
| [GetOperationStatusAndProgressAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_tasks_client.html#ae5096dbaa75b66ccb56dc55bc2ff23e7) | [ListTasksAsync]() |

### TemplateDefinitions
| Laserfiche.Repository.Api.Client | Laserfiche.Repository.Api.Client.V2 |
|----------------------------------|-------------------------------------|
| [GetTemplateDefinitionByIdAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a8500bcc5c6014e1661bfbbed6f19fd78) | [GetTemplateDefinitionAsync]() |
| [GetTemplateDefinitionsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a7a767abbd6393e521a01c4b4f38aad45) | [ListTemplateDefinitionsAsync]() |
| [GetTemplateDefinitionsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#af073a216db209c9ef8a2bbc13f650467) | [ListTemplateDefinitionsForEachAsync]() |
| [GetTemplateDefinitionsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a182c86e8641c1a49b7fbb26c8a353649) | [ListTemplateDefinitionsNextLinkAsync]() |
| [GetTemplateFieldDefinitionsAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#ab39bde6483ce4f5e2248cf347a86e6a9) | [ListTemplateFieldDefinitionsByTemplateIdAsync]() |
| [GetTemplateFieldDefinitionsForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a9d1a024286b6e38a32d4a4ba2ae3cc08) | [ListTemplateFieldDefinitionsByTemplateIdForEachAsync]() |
| [GetTemplateFieldDefinitionsNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#ae4b743d0c678fdb2c735e9ac838fb4ec) | [ListTemplateFieldDefinitionsByTemplateIdNextLinkAsync]() |
| [GetTemplateFieldDefinitionsByTemplateNameAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#aa6ba5132337cc8bdb8f71fc434261fbb) | [ListTemplateFieldDefinitionsByTemplateNameAsync]() |
| [GetTemplateFieldDefinitionsByTemplateNameForEachAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a7fccd1ffd7dbd789bf391d63e8970a11) | [ListTemplateFieldDefinitionsByTemplateNameForEachAsync]() |
| [GetTemplateFieldDefinitionsByTemplateNameNextLinkAsync](https://laserfiche.github.io/lf-repository-api-client-dotnet/docs/1.x/interface_laserfiche_1_1_repository_1_1_api_1_1_client_1_1_i_template_definitions_client.html#a6206c4c82f96a2e7bfc5d28f343670e2) | [ListTemplateFieldDefinitionsByTemplateNameNextLinkAsync]() |
