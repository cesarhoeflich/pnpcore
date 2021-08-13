﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Model;
using PnP.Core.Model.SharePoint;
using PnP.Core.Model.Teams;
using PnP.Core.QueryModel;
using PnP.Core.Test.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PnP.Core.Test.Teams
{
    [TestClass]
    public class TeamChatMessageTests
    {
        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Configure mocking default for all tests in this class, unless override by a specific test
            //TestCommon.Instance.Mocking = false;
        }
                
        [TestMethod]
        public async Task GetChatMessageAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.Channels);
                Assert.IsTrue(team.Channels.Length > 0);

                var channel = team.Channels.AsRequested().FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = "Hello, this is a unit test (GetChatMessageAsyncTest) posting a message - PnP Rocks!";
                await chatMessages.AddAsync(body);
                
                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages.AsRequested();

                var message = updateMessages.First(o => o.Body.Content == body);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }


        [TestMethod]
        public void AddChatMessageTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = context.Team.Get(o => o.Channels);
                Assert.IsTrue(team.Channels.Length > 0);

                var channel = team.Channels.AsRequested().FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channel);

                channel.Load(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageTest) posting a message - PnP Rocks! - Woah...";
                chatMessages.Add(body);
                
                channel = channel.Get(o => o.Messages);
                var updateMessages = channel.Messages.AsRequested();

                var message = updateMessages.First(o => o.Body.Content == body);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public void AddChatMessageOptionsTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = context.Team.Get(o => o.Channels);
                Assert.IsTrue(team.Channels.Length > 0);

                var channel = team.Channels.AsRequested().FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channel);

                channel.Load(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageOptionsTest) posting a message - PnP Rocks! - Woah...";
                var subject = "Options";
                chatMessages.Add(new ChatMessageOptions()
                {
                    Content = body,
                    Subject = subject
                });

                channel = channel.Get(o => o.Messages);
                var updateMessages = channel.Messages.AsRequested();

                var message = updateMessages.First(o => o.Body.Content == body);
                Assert.IsNotNull(message.CreatedDateTime);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public async Task AddChatMessageHtmlAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"<h1>Hello</h1><br />This is a unit test (AddChatMessageHtmlAsyncTest) posting a message - <strong>PnP Rocks!</strong> - Woah...";
                
                await chatMessages.AddAsync(body, ChatMessageContentType.Html);
                
                channel =  await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages;

                var message = updateMessages.AsEnumerable().Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public async Task AddChatMessageFileAttachmentAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // Upload File to SharePoint Library - it will have to remain i guess as onetime upload.
                IFolder folder = await context.Web.Lists.GetByTitle("Documents").RootFolder.GetAsync();
                IFile existingFile = await folder.Files.FirstOrDefaultAsync(o => o.Name == "test_added.docx");
                if(existingFile == default)
                {
                    existingFile = await folder.Files.AddAsync("test_added.docx", System.IO.File.OpenRead($".{Path.DirectorySeparatorChar}TestAssets{Path.DirectorySeparatorChar}test.docx"));
                }
                
                Assert.IsNotNull(existingFile);
                Assert.AreEqual("test_added.docx", existingFile.Name);

                // Useful reference - https://docs.microsoft.com/en-us/graph/api/chatmessage-post?view=graph-rest-beta&tabs=http#example-4-file-attachments
                // assume as if there are no chat messages
                var attachmentId = existingFile.ETag.AsGraphEtag(); // Needs to be the documents eTag - just the GUID part
                var body = $"<h1>Hello</h1><br />This is a unit test with a file attachment (AddChatMessageHtmlAsyncTest) posting a message - <attachment id=\"{attachmentId}\"></attachment>";
                
                var fileUri = new Uri(existingFile.LinkingUrl);

                await chatMessages.AddAsync(new ChatMessageOptions{
                    Content = body,
                    ContentType = ChatMessageContentType.Html,
                    Attachments = {
                        new ChatMessageAttachmentOptions
                        {
                            Id = attachmentId,
                            ContentType = "reference",
                            // Cannot have the extension with a query graph doesnt recognise and think its part of file extension - include in docs.
                            ContentUrl = new Uri(fileUri.ToString().Replace(fileUri.Query, "")),
                            Name = $"{existingFile.Name}",
                            ThumbnailUrl = null,
                            Content = null
                        }
                    }
                });
                
                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages;

                var message = updateMessages.AsEnumerable().Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

                //delete file
                await existingFile.DeleteAsync(); //Note this will break the link in the Teams chat.

            }
        }

        [TestMethod]
        public async Task AddChatMessageInlineImagesAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // Useful reference - https://docs.microsoft.com/en-us/graph/api/chatmessage-post?view=graph-rest-beta&tabs=http#example-5-sending-inline-images-along-with-the-message
                // assume as if there are no chat messages
                
                var body = $"<div><div><h1>Hello</h1><p>This is a unit test (AddChatMessageInlineImagesAsyncTest) posting a message with inline image</p><div><span><img height=\"392\" src=\"../hostedContents/1/$value\" width=\"300\" style=\"vertical-align:bottom; width:300px; height:392px\"></span></div></div></div>";
                
                await chatMessages.AddAsync(new ChatMessageOptions
                {
                    Content = body,
                    ContentType = ChatMessageContentType.Html,
                    HostedContents =
                    {
                        new ChatMessageHostedContentOptions
                        {
                            Id = "1",
                            ContentBytes = "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",
                            ContentType = "image/png"
                        }
                    }
                });
                
                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages;

                var message = updateMessages.AsEnumerable().Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);
                Assert.IsNotNull(message.HostedContents);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public void AddChatMessageInlineImagesBatchTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var batch = context.NewBatch();

                var team = context.Team.GetBatch(batch, o => o.PrimaryChannel);                
                context.Execute(batch);

                var primaryChannel = team.Result.PrimaryChannel;
                Assert.IsNotNull(primaryChannel);

                batch = context.NewBatch();
                var channel = primaryChannel.GetBatch(batch, o => o.Messages);
                context.Execute(batch);

                var chatMessages = channel.Result.Messages;
                Assert.IsNotNull(chatMessages);

                // Useful reference - https://docs.microsoft.com/en-us/graph/api/chatmessage-post?view=graph-rest-beta&tabs=http#example-5-sending-inline-images-along-with-the-message
                // assume as if there are no chat messages

                var body = $"<div><div><h1>Hello</h1><p>This is a unit test (AddChatMessageInlineImagesAsyncTest) posting a message with inline image</p><div><span><img height=\"392\" src=\"../hostedContents/1/$value\" width=\"300\" style=\"vertical-align:bottom; width:300px; height:392px\"></span></div></div></div>";

                ITeamChatMessageHostedContentCollection coll = new TeamChatMessageHostedContentCollection
                {
                    
                };

                batch = context.NewBatch();
                chatMessages.AddBatch(batch, new ChatMessageOptions
                    {
                        Content = body,
                        ContentType = ChatMessageContentType.Html,
                        HostedContents = {
                            new ChatMessageHostedContentOptions
                            {
                                Id = "1",
                                ContentBytes = "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",
                                ContentType = "image/png"
                            }
                        }
                    });

                context.Execute(batch);
                                
                var batch2 = context.NewBatch();

                channel = channel.Result.GetBatch(batch2, o => o.Messages);
                context.Execute(batch2);
                var updateMessages = channel.Result.Messages.AsRequested();

                var message = updateMessages.Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);
                Assert.IsNotNull(message.HostedContents);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public async Task AddChatMessageFileAttachmentOptionsAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // Upload File to SharePoint Library - it will have to remain i guess as onetime upload.
                IFolder folder = await context.Web.Lists.GetByTitle("Documents").RootFolder.GetAsync();
                IFile existingFile = await folder.Files.FirstOrDefaultAsync(o => o.Name == "test_added.docx");
                if (existingFile == default)
                {
                    existingFile = await folder.Files.AddAsync("test_added.docx", System.IO.File.OpenRead($".{Path.DirectorySeparatorChar}TestAssets{Path.DirectorySeparatorChar}test.docx"));
                }

                Assert.IsNotNull(existingFile);
                Assert.AreEqual("test_added.docx", existingFile.Name);

                // Useful reference - https://docs.microsoft.com/en-us/graph/api/chatmessage-post?view=graph-rest-beta&tabs=http#example-4-file-attachments
                // assume as if there are no chat messages
                var attachmentId = existingFile.ETag.AsGraphEtag(); // Needs to be the documents eTag - just the GUID part
                var body = $"<h1>Hello</h1><br />This is a unit test with a file attachment (AddChatMessageHtmlAsyncTest) posting a message - <attachment id=\"{attachmentId}\"></attachment>";
                var subject = "Chat Message File Attachment Options Async Test";
                var fileUri = new Uri(existingFile.LinkingUrl);
                await context.ExecuteAsync();

                var batch = context.NewBatch();

                await chatMessages.AddBatchAsync(batch, new ChatMessageOptions
                {
                    Subject = subject,
                    Content = body,
                    ContentType = ChatMessageContentType.Html,
                    Attachments = {
                        new ChatMessageAttachmentOptions
                        {
                            Id = attachmentId,
                            ContentType = "reference",
                            // Cannot have the extension with a query graph doesnt recognise and think its part of file extension - include in docs.
                            ContentUrl = new Uri(fileUri.ToString().Replace(fileUri.Query, "")),
                            Name = $"{existingFile.Name}",
                            ThumbnailUrl = null,
                            Content = null
                        }
                    }
                });

                await context.ExecuteAsync(batch);

                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages;

                var message = updateMessages.AsEnumerable().OrderByDescending(o=>o.Subject = subject).Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

                //delete file
                await existingFile.DeleteAsync(); //Note this will break the link in the Teams chat.

            }
        }

        [TestMethod]    
        public async Task AddChatMessageSubjectAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                await channel.LoadAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, This is a unit test (AddChatMessageSubjectAsyncTest) posting a message - PnP Rocks! - Woah...";
                await chatMessages.AddAsync(body, subject: "This is a subject test");
               
                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages.AsEnumerable();

                var message = updateMessages.Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                // BERT - Check with PAUL
                //Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                //Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public async Task AddChatMessageThumbnailAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                await channel.LoadAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var attachmentId = "74d20c7f34aa4a7fb74e2b30004247c5";
                var body = $"<attachment id=\"{attachmentId}\"></attachment>";
               
                await chatMessages.AddAsync(new ChatMessageOptions
                    {
                        Content = body,
                        ContentType = ChatMessageContentType.Html,
                        Attachments =
                        {
                             new ChatMessageAttachmentOptions
                            {
                                Id = attachmentId,
                                ContentType = "application/vnd.microsoft.card.thumbnail",
                                // Adaptive Card
                                Content = "{\r\n  \"title\": \"Unit Test posting a card\",\r\n  \"subtitle\": \"<h3>This is the subtitle</h3>\",\r\n  \"text\": \"Here is some body text. <br>\\r\\nAnd a <a href=\\\"http://microsoft.com/\\\">hyperlink</a>. <br>\\r\\nAnd below that is some buttons:\",\r\n  \"buttons\": [\r\n    {\r\n      \"type\": \"messageBack\",\r\n      \"title\": \"Login to FakeBot\",\r\n      \"text\": \"login\",\r\n      \"displayText\": \"login\",\r\n      \"value\": \"login\"\r\n    }\r\n  ]\r\n}",
                                ContentUrl = null,
                                Name = null,
                                ThumbnailUrl = null
                            }
                        }
                    });
                
                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages.AsRequested();

                var message = updateMessages.Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public async Task AddChatMessageAdaptiveAsyncTest()
        {
            //Reference: https://docs.microsoft.com/en-us/microsoftteams/platform/task-modules-and-cards/cards/cards-reference#adaptive-card

            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                var team = await context.Team.GetAsync(o => o.PrimaryChannel);
                var channel = team.PrimaryChannel;
                Assert.IsNotNull(channel);

                channel = await channel.GetAsync(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var attachmentId = "74d20c7f34aa4a7fb74e2b30004247c5";
                var body = $"<attachment id=\"{attachmentId}\"></attachment>";
                
                await chatMessages.AddAsync(new ChatMessageOptions
                {
                    Content = body,
                    ContentType = ChatMessageContentType.Html,
                    Attachments = {
                        new ChatMessageAttachmentOptions
                        {
                            Id = attachmentId,
                            ContentType = "application/vnd.microsoft.card.adaptive",
                            // Adaptive Card
                            Content = "{\"$schema\":\"http://adaptivecards.io/schemas/adaptive-card.json\",\"type\":\"AdaptiveCard\",\"version\":\"1.0\",\"body\":[{\"type\":\"Container\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"Adaptive Card Unit Test\",\"weight\":\"bolder\",\"size\":\"medium\"},{\"type\":\"ColumnSet\",\"columns\":[{\"type\":\"Column\",\"width\":\"auto\",\"items\":[{\"type\":\"Image\",\"url\":\"https://pbs.twimg.com/profile_images/3647943215/d7f12830b3c17a5a9e4afcc370e3a37e_400x400.jpeg\",\"size\":\"small\",\"style\":\"person\"}]},{\"type\":\"Column\",\"width\":\"stretch\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"Matt Hidinger\",\"weight\":\"bolder\",\"wrap\":true},{\"type\":\"TextBlock\",\"spacing\":\"none\",\"text\":\"Created {{DATE(2017-02-14T06:08:39Z,SHORT)}}\",\"isSubtle\":true,\"wrap\":true}]}]}]},{\"type\":\"Container\",\"items\":[{\"type\":\"TextBlock\",\"text\":\"Now that we have defined the main rule sand features of the format ,we need to produce a schema and publish it to GitHub.The schema will be the starting point of our reference documentation.\",\"wrap\":true},{\"type\":\"FactSet\",\"facts\":[{\"title\":\"Board:\",\"value\":\"Adaptive Card\"},{\"title\":\"List:\",\"value\":\"Backlog\"},{\"title\":\"Assigned to:\",\"value\":\"Matt Hidinger\"},{\"title\":\"Duedate:\",\"value\":\"Not set\"}]}]}],\"actions\":[{\"type\":\"Action.ShowCard\",\"title\":\"Set due date\",\"card\":{\"type\":\"AdaptiveCard\",\"body\":[{\"type\":\"Input.Date\",\"id\":\"dueDate\"}],\"actions\":[{\"type\":\"Action.Submit\",\"title\":\"OK\"}]}},{\"type\":\"Action.ShowCard\",\"title\":\"Comment\",\"card\":{\"type\":\"AdaptiveCard\",\"body\":[{\"type\":\"Input.Text\",\"id\":\"comment\",\"isMultiline\":true,\"placeholder\":\"Enter your comment\"}],\"actions\":[{\"type\":\"Action.Submit\",\"title\":\"OK\"}]}}]}",
                            ContentUrl = null,
                            Name = null,
                            ThumbnailUrl = null
                        }
                    }
                });

                channel = await channel.GetAsync(o => o.Messages);
                var updateMessages = channel.Messages.AsEnumerable();

                var message = updateMessages.Last();
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public void AddChatMessageBatchTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = context.Team.GetBatch(o => o.Channels);
                context.Execute();
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = channelQuery.GetBatch(o => o.Messages);
                context.Execute();
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);
                                
                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageBatchTest) posting a message - PnP Rocks! - Woah...";
                
                chatMessages.AddBatch(body, ChatMessageContentType.Text, "Batch Test");
                context.Execute();
                

                channel = channelQuery.GetBatch(o => o.Messages);
                context.Execute();
                var updateMessages = channel.Result.Messages;

                var message = updateMessages.AsRequested().FirstOrDefault(o => o.Body.Content == body);

                Assert.IsFalse(message == default);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public async Task AddChatMessageBatchAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = await context.Team.GetBatchAsync(o => o.Channels);
                await context.ExecuteAsync();
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = await channelQuery.GetBatchAsync(o => o.Messages);
                await context.ExecuteAsync();
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageBatchTest) posting a message - PnP Rocks! - Woah...";
                await chatMessages.AddBatchAsync(body, ChatMessageContentType.Text, "Batch Test");
                await context.ExecuteAsync();
            
                channel = channelQuery.GetBatch(o => o.Messages);
                await context.ExecuteAsync();
                var updateMessages = channel.Result.Messages;

                var message = updateMessages.AsRequested().FirstOrDefault(o => o.Body.Content == body);

                Assert.IsFalse(message == default);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public void AddChatMessageBatchOptionsTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = context.Team.GetBatch(o => o.Channels);
                context.Execute();
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = channelQuery.GetBatch(o => o.Messages);
                context.Execute();
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageBatchTest) posting a message - PnP Rocks! - Woah...";

                chatMessages.AddBatch(new ChatMessageOptions()
                {
                    Content = body,
                    Subject = "ChatMessageBatchOptionsTest"
                });
                context.Execute();


                channel = channelQuery.GetBatch(o => o.Messages);
                context.Execute();
                var updateMessages = channel.Result.Messages;

                var message = updateMessages.AsRequested().FirstOrDefault(o => o.Body.Content == body);

                Assert.IsFalse(message == default);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public async Task AddChatMessageBatchOptionsAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = await context.Team.GetBatchAsync(o => o.Channels);
                await context.ExecuteAsync();
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = await channelQuery.GetBatchAsync(o => o.Messages);
                await context.ExecuteAsync();
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageBatchTest) posting a message - PnP Rocks! - Woah...";
                await chatMessages.AddBatchAsync(new ChatMessageOptions()
                {
                    Content = body,
                    Subject = "AddChatMessageBatchOptionsAsyncTest"
                });
                await context.ExecuteAsync();

                channel = channelQuery.GetBatch(o => o.Messages);
                await context.ExecuteAsync();
                var updateMessages = channel.Result.Messages;

                var message = updateMessages.AsRequested().FirstOrDefault(o => o.Body.Content == body);

                Assert.IsFalse(message == default);
                Assert.IsNotNull(message.CreatedDateTime);
                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);

            }
        }

        [TestMethod]
        public void AddChatMessageSpecificBatchTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var batch = context.NewBatch();
                var team = context.Team.GetBatch(batch, o => o.Channels);
                context.Execute(batch);
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = channelQuery.GetBatch(batch, o => o.Messages);
                context.Execute(batch);
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageSpecificBatchTest) posting a message - PnP Rocks! - Woah...";
                chatMessages.AddBatch(batch, body);
                context.Execute(batch);
                
                var batch2 = context.NewBatch();
                channel = channelQuery.GetBatch(batch2, o => o.Messages);
                context.Execute(batch2);
                var updateMessages = channel.Result.Messages.AsRequested();

                var message = updateMessages.First(o => o.Body.Content == body);
                Assert.IsNotNull(message.CreatedDateTime);

                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        public async Task AddChatMessageSpecificBatchAsyncTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var batch = context.NewBatch();
                var team = await context.Team.GetBatchAsync(batch, o => o.Channels);
                await context.ExecuteAsync(batch);
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = await channelQuery.GetBatchAsync(batch, o => o.Messages);
                await context.ExecuteAsync(batch);
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = $"Hello, this is a unit test (AddChatMessageSpecificBatchAsyncTest) posting a message - PnP Rocks! - Woah...";
                await chatMessages.AddBatchAsync(batch, body, ChatMessageContentType.Text, "Batch Async Test");
                await context.ExecuteAsync(batch);
                

                var batch2 = context.NewBatch();
                channel = await channelQuery.GetBatchAsync(batch2, o => o.Messages);
                await context.ExecuteAsync(batch2);
                var updateMessages = channel.Result.Messages.AsRequested();

                var message = updateMessages.First(o => o.Body.Content == body);
                Assert.IsNotNull(message.CreatedDateTime);

                // Depending on regional settings this check might fail
                //Assert.AreEqual(message.DeletedDateTime, DateTime.MinValue);
                Assert.IsNotNull(message.Etag);
                Assert.IsNotNull(message.Importance);
                Assert.IsNotNull(message.LastModifiedDateTime);
                Assert.IsNotNull(message.Locale);
                Assert.IsNotNull(message.MessageType);
                Assert.IsNotNull(message.WebUrl);

                Assert.IsTrue(message.IsPropertyAvailable(o => o.ReplyToId));
                Assert.IsNull(message.ReplyToId);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Subject));
                Assert.IsNotNull(message.Subject);
                Assert.IsTrue(message.IsPropertyAvailable(o => o.Summary));
                Assert.IsNull(message.Summary);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddChatMessageBatchOptionsExceptionTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var batch = context.NewBatch();
                var team = context.Team.GetBatch(batch, o => o.Channels);
                context.Execute(batch);
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = channelQuery.GetBatch(batch, o => o.Messages);
                context.Execute(batch);
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = string.Empty;
                chatMessages.AddBatch(batch, options: null);
                context.Execute(batch);

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddChatMessageBatchExceptionTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var batch = context.NewBatch();
                var team = context.Team.GetBatch(batch, o => o.Channels);
                context.Execute(batch);
                Assert.IsTrue(team.Result.Channels.Length > 0);

                var channelQuery = team.Result.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channelQuery);

                var channel = channelQuery.GetBatch(batch, o => o.Messages);
                context.Execute(batch);
                var chatMessages = channel.Result.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = string.Empty;
                chatMessages.AddBatch(batch, body);
                context.Execute(batch);
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddChatMessageExceptionTest()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
            {
                var team = context.Team.Get(o => o.Channels);
                Assert.IsTrue(team.Channels.Length > 0);

                var channel = team.Channels.FirstOrDefault(i => i.DisplayName == "General");
                Assert.IsNotNull(channel);

                channel = channel.Get(o => o.Messages);
                var chatMessages = channel.Messages;

                Assert.IsNotNull(chatMessages);

                // assume as if there are no chat messages
                // There appears to be no remove option yet in this feature - so add a recognisable message
                var body = string.Empty;
                chatMessages.Add(body);
                
            }
        }

        //TODO: There is no option to add reactions within a chat message in the SDK therefore cannot automate the testing for this at this time.
        //[TestMethod]
        //public void AddChatMessageReactionTest()
        //{
        //    //TestCommon.Instance.Mocking = false;
        //    using (var context = TestCommon.Instance.GetContext(TestCommon.TestSite))
        //    {
        //        var team = context.Team.Get(o => o.Channels);
        //        Assert.IsTrue(team.Channels.Length > 0);

        //        var channel = team.Channels.FirstOrDefault(i => i.DisplayName == "General");
        //        Assert.IsNotNull(channel);

        //        channel = channel.Get(o => o.Messages);
        //        var chatMessages = channel.Messages;

        //        Assert.IsNotNull(chatMessages);

        //        // assume as if there are no chat messages
        //        // There appears to be no remove option yet in this feature - so add a recognisable message
        //        var body = $"Hello, this is a unit test (AddChatMessageReactionTest) posting a message - PnP Rocks! - Woah...";
        //        var result = chatMessages.Add(body);

                
        //    }
        //}
    }
}
