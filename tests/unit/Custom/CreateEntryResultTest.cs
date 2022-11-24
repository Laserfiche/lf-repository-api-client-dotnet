using System.Collections.Generic;
using Xunit;

namespace Laserfiche.Repository.Api.Client.Test.Custom
{
    public class CreateEntryResultTest
    {
        [Fact]
        public void GetSummary_AllOperationsHaveErrorMessages()
        {
            int entryId = 123;
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
                        EntryId = entryId,
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

            string result = createEntryResult.GetSummary();

            var errorMessages = new string[] { entryCreateErrorMessage, setEdocErrorMessage, setTemplateErrorMessage,
                setFieldsErrorMessage1, setTagsErrorMessage, setLinksErrorMessage };
            string expectedMessage = $"{nameof(createEntryResult.Operations.EntryCreate.EntryId)}={entryId}. " + string.Join(" ", errorMessages);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void GetSummary_OperationHasMultipleErrorMessages()
        {
            string setTemplateErrorMessage1 = "Error setting template1.";
            string setTemplateErrorMessage2 = "Error setting template2.";
            string setTagsErrorMessage1 = "Error setting tag1.";
            string setTagsErrorMessage2 = "Error setting tag2.";
            string setTagsErrorMessage3 = "Error setting tag3.";
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
                    SetFields = new SetFields(),
                    SetTags = new SetTags()
                    {
                        Exceptions = new List<APIServerException>()
                        {
                            new APIServerException() { Message = setTagsErrorMessage1 },
                            new APIServerException() { Message = setTagsErrorMessage2 },
                            new APIServerException() { Message = setTagsErrorMessage3 },
                        }
                    },
                    SetLinks = new SetLinks()
                }
            };

            string result = createEntryResult.GetSummary();

            var errorMessages = new string[] { setTemplateErrorMessage1, setTemplateErrorMessage2,
                setTagsErrorMessage1, setTagsErrorMessage2, setTagsErrorMessage3 };
            string expectedMessage = string.Join(" ", errorMessages);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void GetSummary_NullOperations()
        {
            CreateEntryResult createEntryResult = new CreateEntryResult() { Operations = null };

            string result = createEntryResult.GetSummary();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetSummary_NullEntryOperations()
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

            string result = createEntryResult.GetSummary();

            Assert.Equal(setTemplateErrorMessage, result);
        }

        [Fact]
        public void GetSummary_OperationExceptionWithoutMessage()
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

            string result = createEntryResult.GetSummary();

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetSummary_NoErrors()
        {
            int entryId = 123;
            CreateEntryResult createEntryResult = new CreateEntryResult()
            {
                Operations = new CreateEntryOperations()
                {
                    EntryCreate = new EntryCreate()
                    {
                        EntryId = entryId
                    },
                    SetEdoc = new SetEdoc(),
                    SetTemplate = new SetTemplate(),
                    SetFields = new SetFields(),
                    SetTags = new SetTags(),
                    SetLinks = new SetLinks()
                }
            };

            string result = createEntryResult.GetSummary();

            Assert.Equal($"{nameof(createEntryResult.Operations.EntryCreate.EntryId)}={entryId}.", result);
        }
    }
}
