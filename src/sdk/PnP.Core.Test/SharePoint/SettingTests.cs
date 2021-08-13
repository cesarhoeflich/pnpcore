﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PnP.Core.Model.SharePoint;
using PnP.Core.QueryModel;
using PnP.Core.Services;
using PnP.Core.Test.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PnP.Core.Test.SharePoint
{
    [TestClass]
    public class SettingTests
    {

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            // Configure mocking default for all tests in this class, unless override by a specific test
            //TestCommon.Instance.Mocking = false;

            //Reference
            // b6917cb1-93a0-4b97-a84d-7cf49975d4ec - SitePages - Read this only - modern sites already have this activated
            // 3bae86a2-776d-499d-9db8-fa4cdc7884f8 - SharePoint Viewers - Web Scoped
            // 3bae86a2-776d-499d-9db8-fa4cdc7884f8 - Document Sets - Site Scoped
        }

        [TestMethod]
        public async Task GetFeaturesWebAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);
                Assert.IsTrue(web.Features.Length > 0);
            }
        }

        [TestMethod]
        public async Task GetFeaturesSiteAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                ISite site = await context.Site.GetAsync(p => p.Features);
                Assert.IsTrue(site.Features.Length > 0);
            }
        }

        [TestMethod]
        public async Task EnableWebFeatureAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {

                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                IFeature feature = await web.Features.EnableAsync(id);

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);

                // Ensure the feature is disabled again
                await web.Features.DisableAsync(id);
            }
        }

        [TestMethod]
        public async Task EnableDisableWebFeatureActivateBatchAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                if (web.Features.AsRequested().Any(o => o.DefinitionId == id))
                {
                    // Ensure disabled
                    web.Features.Disable(id);
                }

                IFeature feature = await web.Features.EnableBatchAsync(id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);

                await web.Features.DisableBatchAsync(id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsTrue(!web.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

        [TestMethod]
        public async Task EnableDisableWebFeatureActivateBatch()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                if (web.Features.AsRequested().Any(o => o.DefinitionId == id))
                {
                    // Ensure disabled
                    web.Features.Disable(id);
                }

                IFeature feature = web.Features.EnableBatch(id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);

                web.Features.DisableBatch(id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsTrue(!web.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

        [TestMethod]
        public async Task EnableDisableWebFeatureActivateSpecifiedBatch()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                if (web.Features.AsRequested().Any(o => o.DefinitionId == id))
                {
                    // Ensure disabled
                    web.Features.Disable(id);
                }

                Batch newBatch = context.NewBatch();

                IFeature feature = web.Features.EnableBatch(newBatch, id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);

                web.Features.DisableBatch(newBatch, id);
                await context.ExecuteAsync(); //Trigger Batch

                Assert.IsTrue(!web.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

        [TestMethod]
        public async Task EnableDisableWebFeatureActivateExceptionsBatchAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f740000"); // Fake

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
                {
                    await web.Features.EnableBatchAsync(id);
                    await web.Features.EnableBatchAsync(id);
                    await context.ExecuteAsync(); //Trigger Batch

                });

            }
        }

        [TestMethod]
        public async Task EnableWebFeatureAlreadyActivatedAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                if (web.Features.AsRequested().Any(o => o.DefinitionId == id))
                {
                    // Already Activated
                }
                else
                {
                    // Not Activated lets activate 
                    IFeature feature = await web.Features.EnableAsync(id);

                    Assert.IsNotNull(feature);
                    Assert.IsNotNull(feature.DefinitionId);
                    Assert.IsTrue(feature.DefinitionId != Guid.Empty);
                }

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
                {
                    await web.Features.EnableAsync(id);
                });

                // Deactivate again
                await web.Features.DisableAsync(id);
            }
        }

        [TestMethod]
        public async Task DisableWebFeatureDoesNotExistAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f700000"); // fake

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
                {
                    await web.Features.DisableAsync(id);
                });
            }
        }

        [TestMethod]
        public async Task EnableDisableWebFeature()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                // Not Activated lets activate 
                IFeature feature = web.Features.Enable(id);

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);


                web.Features.Disable(id);

                Assert.IsTrue(!web.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

        [TestMethod]
        public async Task DisableWebFeatureAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {

                IWeb web = await context.Web.GetAsync(p => p.Features);

                var id = new Guid("fa6a1bcc-fb4b-446b-8460-f4de5f7411d5"); // SharePoint Viewers - Web Scoped

                // Enable first
                await web.Features.EnableAsync(id);

                await web.Features.DisableAsync(id);

                Assert.IsTrue(!web.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

        [TestMethod]
        public async Task EnableSiteFeatureAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                ISite site = await context.Site.GetAsync(p => p.Features);

                var id = new Guid("3bae86a2-776d-499d-9db8-fa4cdc7884f8"); // Document Sets - Site Scoped
                IFeature feature = await site.Features.EnableAsync(id);

                Assert.IsNotNull(feature);
                Assert.IsNotNull(feature.DefinitionId);
                Assert.IsTrue(feature.DefinitionId != Guid.Empty);

                // Disable the feature again
                await site.Features.DisableAsync(id);
            }
        }

        [TestMethod]
        public async Task DisableSiteFeatureAsync()
        {
            //TestCommon.Instance.Mocking = false;
            using (var context = await TestCommon.Instance.GetContextAsync(TestCommon.TestSite))
            {
                ISite site = await context.Site.GetAsync(p => p.Features);

                var id = new Guid("3bae86a2-776d-499d-9db8-fa4cdc7884f8"); // Document Sets - Site Scoped
                // first enable
                await site.Features.EnableAsync(id);


                await site.Features.DisableAsync(id);

                Assert.IsTrue(!site.Features.AsRequested().Any(o => o.DefinitionId == id));
            }
        }

    }
}
