using System.Collections.Generic;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class CreateEntryResultTest
    {
        [Fact]
        public void GetErrorMessages_AllOperationsHaveErrorMessages()
        {
            string entryCreateErrorMessage = "Error creating entry.";
            string setEdocErrorMessage = "Error setting edoc.";
            string setTemplateErrorMessage = "Error setting template.";
            string setFieldsErrorMessage1 = "Error setting field1.";
            string setTagsErrorMessage = "Error setting tag.";
            string setLinksErrorMessage = "Error setting link.";
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = new EntryCreate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = entryCreateErrorMessage }
                        }
                    },
                    SetEdoc = new SetEdoc()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setEdocErrorMessage }
                        }
                    },
                    SetTemplate = new SetTemplate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTemplateErrorMessage }
                        }
                    },
                    SetFields = new SetFields()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setFieldsErrorMessage1 }
                        }
                    },
                    SetTags = new SetTags()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTagsErrorMessage }
                        }
                    },
                    SetLinks = new SetLinks()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setLinksErrorMessage }
                        }
                    }
                }
            };

            string result = createEntryResult.GetErrorMessages();

            string expectedErrorMessage = entryCreateErrorMessage + setEdocErrorMessage + setTemplateErrorMessage + 
                setFieldsErrorMessage1 + setTagsErrorMessage + setLinksErrorMessage;
            Assert.Equal(expectedErrorMessage, result);
        }

        [Fact]
        public void GetErrorMessages_OperationHasMultipleErrorMessages()
        {
            string setTemplateErrorMessage1 = "Error setting template1.";
            string setTemplateErrorMessage2 = "Error setting template2.";
            string setFieldsErrorMessage1 = "Error setting field1.";
            string setFieldsErrorMessage2 = "Error setting field2.";
            string setFieldsErrorMessage3 = "Error setting field3.";
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = new EntryCreate(),
                    SetEdoc = new SetEdoc(),
                    SetTemplate = new SetTemplate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTemplateErrorMessage1 },
                            new APIServerException() { Message = setTemplateErrorMessage2 },
                        }
                    },
                    SetFields = new SetFields()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setFieldsErrorMessage1 },
                            new APIServerException() { Message = setFieldsErrorMessage2 },
                            new APIServerException() { Message = setFieldsErrorMessage3 },
                        }
                    },
                    SetTags = new SetTags(),
                    SetLinks = new SetLinks()
                }
            };

            string result = createEntryResult.GetErrorMessages();

            string expectedErrorMessage = setTemplateErrorMessage1 + setTemplateErrorMessage2 + setFieldsErrorMessage1 + setFieldsErrorMessage2 + setFieldsErrorMessage3;
            Assert.Equal(expectedErrorMessage, result);
        }

        [Fact]
        public void GetErrorMessages_NullOperations()
        {
            CreateEntryResult createEntryResult = new CreateEntryResult() { Operations = null };

            string result = createEntryResult.GetErrorMessages();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetErrorMessages_NullEntryOperations()
        {
            string setTemplateErrorMessage = "Error setting template.";
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = null,
                    SetEdoc = null,
                    SetTemplate = new SetTemplate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTemplateErrorMessage }
                        }
                    },
                    SetFields = null,
                    SetTags = null,
                    SetLinks = null
                }
            };

            string result = createEntryResult.GetErrorMessages();

            Assert.Equal(setTemplateErrorMessage, result);
        }

        [Fact]
        public void GetErrorMessages_OperationExceptionWithoutMessage()
        {
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = null,
                    SetEdoc = null,
                    SetTemplate = new SetTemplate()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException()
                        }
                    },
                    SetFields = null,
                    SetTags = null,
                    SetLinks = null
                }
            };

            string result = createEntryResult.GetErrorMessages();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetErrorMessages_NoErrors()
        {
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = new EntryCreate()
                    {
                        EntryId = 123
                    },
                    SetEdoc = new SetEdoc(),
                    SetTemplate = new SetTemplate(),
                    SetFields = new SetFields(),
                    SetTags = new SetTags(),
                    SetLinks = new SetLinks()
                }
            };

            string result = createEntryResult.GetErrorMessages();

            Assert.Equal(string.Empty, result);
        }
    }
}
