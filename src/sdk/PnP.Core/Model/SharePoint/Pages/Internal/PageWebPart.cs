﻿using AngleSharp.Dom;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// This class is used to instantiate controls of type 3 (= client side web parts). Using this class you can instantiate a control and 
    /// add it on a <see cref="IPage"/>.
    /// </summary>
    internal class PageWebPart : CanvasControl, IPageWebPart
    {
        #region variables
        // Constants
        internal const string WebPartAttribute = "data-sp-webpart";
        internal const string WebPartDataVersionAttribute = "data-sp-webpartdataversion";
        internal const string WebPartDataAttribute = "data-sp-webpartdata";
        internal const string WebPartComponentIdAttribute = "data-sp-componentid";
        internal const string WebPartHtmlPropertiesAttribute = "data-sp-htmlproperties";

        private string propertiesJson;
        #endregion

        #region construction
        /// <summary>
        /// Instantiates client side web part from scratch.
        /// </summary>
        public PageWebPart() : base()
        {
            controlType = 3;
            WebPartData = "";
            HtmlPropertiesData = "";
            HtmlProperties = "";
            Title = "";
            Description = "";
            SupportsFullBleed = false;
            SetPropertiesJson(JsonDocument.Parse("{}").RootElement);
            WebPartPreviewImage = "";
            UsingSpControlDataOnly = false;
            DynamicDataPaths = JsonDocument.Parse("{}").RootElement;
            DynamicDataValues = JsonDocument.Parse("{}").RootElement;
            ServerProcessedContent = JsonDocument.Parse("{}").RootElement;
        }

        /// <summary>
        /// Instantiates a client side web part based on the information that was obtain from calling the AvailableClientSideComponents methods on the <see cref="IPage"/> object.
        /// </summary>
        /// <param name="component">Component to create a ClientSideWebPart instance for</param>
        public PageWebPart(IPageComponent component) : this()
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            Import(component as PageComponent);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Value of the "data-sp-webpartdata" attribute
        /// </summary>
        public string JsonWebPartData { get; private set; }

        /// <summary>
        /// Value of the "data-sp-htmlproperties" element
        /// </summary>
        public string HtmlPropertiesData { get; private set; }

        /// <summary>
        /// Value of the "data-sp-htmlproperties" attribute
        /// </summary>
        public string HtmlProperties { get; private set; }

        /// <summary>
        /// ID of the client side web part
        /// </summary>
        public string WebPartId { get; private set; }

        /// <summary>
        /// Supports full bleed display experience
        /// </summary>
        public bool SupportsFullBleed { get; private set; }

        /// <summary>
        /// Value of the "data-sp-webpart" attribute
        /// </summary>
        public string WebPartData { get; private set; }

        /// <summary>
        /// Title of the web part
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the web part
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Preview image that can serve as page preview image when the page holding this web part is promoted to a news page
        /// </summary>
        public string WebPartPreviewImage { get; private set; }

        /// <summary>
        /// Json serialized web part information. For 1st party web parts this ideally is the *full* JSON string 
        /// fetch via workbench or via copying it from an existing page. It's important that the serverProcessedContent
        /// element is included here!
        /// </summary>
        public string PropertiesJson
        {
            get
            {
                return Properties.ToString();
            }
            set
            {
                SetPropertiesJson(JsonDocument.Parse(value).RootElement);
            }
        }

        /// <summary>
        /// Web properties as configurable <see cref="JsonElement"/>
        /// </summary>
        public JsonElement Properties { get; private set; }

        /// <summary>
        /// ServerProcessedContent json node
        /// </summary>
        public JsonElement ServerProcessedContent { get; private set; }

        //public JObject DynamicDataPaths
        public JsonElement DynamicDataPaths { get; private set; }

        //public JObject DynamicDataValues
        public JsonElement DynamicDataValues { get; private set; }

        /// <summary>
        /// Return <see cref="Type"/> of the client side web part
        /// </summary>
        public override Type Type
        {
            get
            {
                return typeof(PageWebPart);
            }
        }

        /// <summary>
        /// Value of the "data-sp-controldata" attribute
        /// </summary>
        public WebPartControlData SpControlData { get; private set; }

        /// <summary>
        /// Indicates that this control is persisted/read using the data-sp-controldata attribute only
        /// </summary>
        internal bool UsingSpControlDataOnly { get; set; }

        /// <summary>
        /// This control lives in the page header (not removable control)
        /// </summary>
        public bool IsHeaderControl { get; internal set; }

        #endregion

        #region public methods
        /// <summary>
        /// Imports a <see cref="PageComponent"/> to use it as base for configuring the client side web part instance
        /// </summary>
        /// <param name="component"><see cref="PageComponent"/> to import</param>
        /// <param name="clientSideWebPartPropertiesUpdater">Function callback that allows you to manipulate the client side web part properties after import</param>
        public void Import(PageComponent component, Func<string, string> clientSideWebPartPropertiesUpdater = null)
        {
            // Sometimes the id guid is encoded with curly brackets, so let's drop those
            WebPartId = new Guid(component.Id).ToString("D");

            // Parse the manifest json blob as we need some data from it
            var wpJObject = JsonDocument.Parse(component.Manifest).RootElement;

            Title = wpJObject.GetProperty("preconfiguredEntries").EnumerateArray().First().GetProperty("title").GetProperty("default").GetString();

            Description = wpJObject.GetProperty("preconfiguredEntries").EnumerateArray().First().GetProperty("title").GetProperty("default").GetString();

            if (wpJObject.TryGetProperty("supportsFullBleed", out JsonElement supportsFullBleed))
            {
                SupportsFullBleed = supportsFullBleed.GetBoolean();
            }
            else
            {
                SupportsFullBleed = false;
            }

            SetPropertiesJson(wpJObject.GetProperty("preconfiguredEntries").EnumerateArray().First().GetProperty("properties"));

            if (clientSideWebPartPropertiesUpdater != null)
            {
                propertiesJson = clientSideWebPartPropertiesUpdater(propertiesJson);
            }
        }

        /// <summary>
        /// Returns a HTML representation of the client side web part
        /// </summary>
        /// <param name="controlIndex">The sequence of the control inside the section</param>
        /// <returns>HTML representation of the client side web part</returns>
        public override string ToHtml(float controlIndex)
        {
            if (!IsHeaderControl)
            {
                // Can this control be hosted in this section type?
                if (Section.Type == CanvasSectionTemplate.OneColumnFullWidth)
                {
                    if (!SupportsFullBleed)
                    {
                        throw new ClientException(ErrorType.Unsupported, PnPCoreResources.Exception_Page_ControlNotAllowedInFullWidthSection);
                    }
                }

                WebPartControlData controlData;
                if (UsingSpControlDataOnly)
                {
                    controlData = new WebPartControlDataOnly();
                }
                else
                {
                    controlData = new WebPartControlData();
                }

                // Obtain the json data
                controlData.ControlType = ControlType;
                controlData.Id = InstanceId.ToString("D");
                controlData.WebPartId = WebPartId;
                controlData.Position = new CanvasControlPosition()
                {
                    ZoneIndex = Section.Order,
                    SectionIndex = Column.Order,
                    SectionFactor = Column.ColumnFactor,
                    LayoutIndex = Column.LayoutIndex,
                    ControlIndex = controlIndex,
                };

                if (section.Type == CanvasSectionTemplate.OneColumnVerticalSection)
                {
                    if (section.Columns.First().Equals(Column))
                    {
                        controlData.Position.SectionFactor = 12;
                    }
                }

                controlData.Emphasis = new SectionEmphasis()
                {
                    ZoneEmphasis = Column.VerticalSectionEmphasis ?? Section.ZoneEmphasis,
                };

                // Set the control's data version to the latest version...default was 1.0, but some controls use a higher version
                var webPartType = Page.IdToDefaultWebPart(controlData.WebPartId);

                // if we read the control from the page then the value might already be set to something different than 1.0...if so, leave as is
                if (DataVersion == "1.0")
                {
                    if (webPartType == DefaultWebPart.Image)
                    {
                        dataVersion = "1.9";
                    }
                    else if (webPartType == DefaultWebPart.ImageGallery)
                    {
                        dataVersion = "1.8";
                    }
                    else if (webPartType == DefaultWebPart.People)
                    {
                        dataVersion = "1.3";
                    }
                    else if (webPartType == DefaultWebPart.DocumentEmbed)
                    {
                        dataVersion = "1.2";
                    }
                    else if (webPartType == DefaultWebPart.ContentRollup)
                    {
                        dataVersion = "2.5";
                    }
                    else if (webPartType == DefaultWebPart.QuickLinks)
                    {
                        dataVersion = "2.2";
                    }
                }

                // Set the web part preview image url
                if (ServerProcessedContent.TryGetProperty("imageSources", out JsonElement imageSources))
                {
                    foreach (var property in imageSources.EnumerateObject())
                    {
                        if (!string.IsNullOrEmpty(property.Value.ToString()))
                        {
                            WebPartPreviewImage = property.Value.ToString().ToLower();
                            break;
                        }
                    }
                }

                WebPartData webpartData = new WebPartData() { Id = controlData.WebPartId, InstanceId = controlData.Id, Title = Title, Description = Description, DataVersion = DataVersion, Properties = "jsonPropsToReplacePnPRules", DynamicDataPaths = "jsonDynamicDataPathsToReplacePnPRules", DynamicDataValues = "jsonDynamicDataValuesToReplacePnPRules", ServerProcessedContent = "jsonServerProcessedContentToReplacePnPRules" };

                if (UsingSpControlDataOnly)
                {
                    (controlData as WebPartControlDataOnly).WebPartData = "jsonWebPartDataToReplacePnPRules";
                    jsonControlData = JsonSerializer.Serialize(controlData);
                    JsonWebPartData = JsonSerializer.Serialize(webpartData);
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonPropsToReplacePnPRules\"", Properties.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonServerProcessedContentToReplacePnPRules\"", ServerProcessedContent.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonDynamicDataPathsToReplacePnPRules\"", DynamicDataPaths.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonDynamicDataValuesToReplacePnPRules\"", DynamicDataValues.ToString());
                    jsonControlData = jsonControlData.Replace("\"jsonWebPartDataToReplacePnPRules\"", JsonWebPartData);
                }
                else
                {
                    jsonControlData = JsonSerializer.Serialize(controlData);
                    JsonWebPartData = JsonSerializer.Serialize(webpartData);
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonPropsToReplacePnPRules\"", Properties.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonServerProcessedContentToReplacePnPRules\"", ServerProcessedContent.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonDynamicDataPathsToReplacePnPRules\"", DynamicDataPaths.ToString());
                    JsonWebPartData = JsonWebPartData.Replace("\"jsonDynamicDataValuesToReplacePnPRules\"", DynamicDataValues.ToString());
                }
            }
            else
            {
                HeaderControlData webpartData = new HeaderControlData() { Id = WebPartId, InstanceId = InstanceId.ToString("D"), Title = Title, Description = Description, DataVersion = DataVersion, Properties = "jsonPropsToReplacePnPRules", ServerProcessedContent = "jsonServerProcessedContentToReplacePnPRules" };
                canvasDataVersion = DataVersion;
                JsonWebPartData = JsonSerializer.Serialize(webpartData);
                JsonWebPartData = JsonWebPartData.Replace("\"jsonPropsToReplacePnPRules\"", Properties.ToString());
                JsonWebPartData = JsonWebPartData.Replace("\"jsonServerProcessedContentToReplacePnPRules\"", ServerProcessedContent.ToString());
                jsonControlData = JsonWebPartData;
            }

            StringBuilder html = new StringBuilder(100);
            if (UsingSpControlDataOnly || IsHeaderControl)
            {
                html.Append($@"<div {CanvasControlAttribute}=""{CanvasControlData}"" {CanvasDataVersionAttribute}=""{DataVersion}"" {ControlDataAttribute}=""{JsonControlData.Replace("\"", "&quot;")}""></div>");
            }
            else
            {
                html.Append($@"<div {CanvasControlAttribute}=""{CanvasControlData}"" {CanvasDataVersionAttribute}=""{DataVersion}"" {ControlDataAttribute}=""{JsonControlData.Replace("\"", "&quot;")}"">");
                html.Append($@"<div {WebPartAttribute}=""{WebPartData}"" {WebPartDataVersionAttribute}=""{DataVersion}"" {WebPartDataAttribute}=""{JsonWebPartData.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;")}"">");
                html.Append($@"<div {WebPartComponentIdAttribute}="""">");
                html.Append(WebPartId);
                html.Append("</div>");
                html.Append($@"<div {WebPartHtmlPropertiesAttribute}=""{HtmlProperties}"">");
                RenderHtmlProperties(ref html);
                html.Append("</div>");
                html.Append("</div>");
                html.Append("</div>");
            }
            return html.ToString();
        }

        /// <summary>
        /// Overrideable method that allows inheriting webparts to control the HTML rendering
        /// </summary>
        /// <param name="htmlWriter">Reference to the html renderer used</param>
        protected virtual void RenderHtmlProperties(ref StringBuilder htmlWriter)
        {
            if (!ServerProcessedContent.Equals(default))
            {
                if (ServerProcessedContent.TryGetProperty("searchablePlainTexts", out JsonElement searchablePlainTexts))
                {
                    foreach (var property in searchablePlainTexts.EnumerateObject())
                    {
                        htmlWriter.Append($@"<div data-sp-prop-name=""{property.Name}"" data-sp-searchableplaintext=""true"">");
                        htmlWriter.Append(property.Value.GetString());
                        htmlWriter.Append("</div>");
                    }
                }

                if (ServerProcessedContent.TryGetProperty("imageSources", out JsonElement imageSources))
                {
                    foreach (var property in imageSources.EnumerateObject())
                    {
                        htmlWriter.Append($@"<img data-sp-prop-name=""{property.Name}""");

                        if (!string.IsNullOrEmpty(property.Value.ToString()))
                        {
                            htmlWriter.Append($@" src=""{property.Value.GetString()}""");
                        }
                        htmlWriter.Append("></img>");
                    }
                }

                if (ServerProcessedContent.TryGetProperty("links", out JsonElement links))
                {
                    if (links.ValueKind != JsonValueKind.Null)
                    {
                        foreach (var property in links.EnumerateObject())
                        {
                            htmlWriter.Append($@"<a data-sp-prop-name=""{property.Name}"" href=""{property.Value.GetString()}""></a>");
                        }
                    }
                }

                if (ServerProcessedContent.TryGetProperty("htmlStrings", out JsonElement htmlStrings))
                {
                    foreach (var property in htmlStrings.EnumerateObject())
                    {
                        htmlWriter.Append($@"<div data-sp-prop-name=""{property.Name}"">{property.Value.GetString()}</div>");
                    }
                }
            }
            else
            {
                htmlWriter.Append(HtmlPropertiesData);
            }
        }
        #endregion

        #region Internal and private methods
        internal override void FromHtml(IElement element)
        {
            base.FromHtml(element);

            // Set/update dataVersion if it was provided as html attribute
            var webPartDataVersion = element.GetAttribute(WebPartDataVersionAttribute);
            if (!string.IsNullOrEmpty(webPartDataVersion))
            {
                dataVersion = element.GetAttribute(WebPartDataVersionAttribute);
            }

            SpControlData = JsonSerializer.Deserialize<WebPartControlData>(element.GetAttribute(ControlDataAttribute), new JsonSerializerOptions() { IgnoreNullValues = true });
            controlType = SpControlData.ControlType;

            var wpDiv = element.GetElementsByTagName("div").Where(a => a.HasAttribute(WebPartDataAttribute)).FirstOrDefault();

            string decodedWebPart;
            // Some components are in the page header and need to be handled as a control instead of a webpart
            if (wpDiv == null)
            {
                // Decode the html encoded string
                decodedWebPart = WebUtility.HtmlDecode(element.GetAttribute(ControlDataAttribute));
                IsHeaderControl = true;
            }
            else
            {
                WebPartData = wpDiv.GetAttribute(WebPartAttribute);

                // Decode the html encoded string
                decodedWebPart = WebUtility.HtmlDecode(wpDiv.GetAttribute(WebPartDataAttribute));
            }

            var wpJObject = JsonDocument.Parse(decodedWebPart).RootElement;

            if (wpJObject.TryGetProperty("title", out JsonElement titleProperty))
            {
                Title = titleProperty.GetString();
            }
            else
            {
                Title = "";
            }

            if (wpJObject.TryGetProperty("description", out JsonElement descriptionProperty))
            {
                Description = descriptionProperty.GetString();
            }
            else
            {
                Description = "";
            }

            // Set property to trigger correct loading of properties 
            PropertiesJson = wpJObject.GetProperty("properties").ToString();

            // Set/update dataVersion if it was set in the json data
            if (wpJObject.TryGetProperty("dataVersion", out JsonElement dataVersionValue))
            {
                dataVersion = dataVersionValue.GetString();
            }

            // Check for fullbleed supporting web parts
            if (wpJObject.TryGetProperty("properties", out JsonElement properties))
            {
                if (properties.TryGetProperty("isFullWidth", out JsonElement isFullWidth))
                {
                    SupportsFullBleed = isFullWidth.GetBoolean();
                }
            }

            // Store the server processed content as that's needed for full fidelity
            if (wpJObject.TryGetProperty("serverProcessedContent", out JsonElement serverProcessedContent))
            {
                ServerProcessedContent = serverProcessedContent;
            }

            if (wpJObject.TryGetProperty("dynamicDataPaths", out JsonElement dynamicDataPaths))
            {
                DynamicDataPaths = dynamicDataPaths;
            }

            if (wpJObject.TryGetProperty("dynamicDataValues", out JsonElement dynamicDataValues))
            {
                DynamicDataValues = dynamicDataValues;
            }

            WebPartId = wpJObject.GetProperty("id").GetString();

            if (wpDiv != null)
            {
                var wpHtmlProperties = wpDiv.GetElementsByTagName("div").Where(a => a.HasAttribute(WebPartHtmlPropertiesAttribute)).FirstOrDefault();
                HtmlPropertiesData = wpHtmlProperties.InnerHtml;
                HtmlProperties = wpHtmlProperties.GetAttribute(WebPartHtmlPropertiesAttribute);
            }
        }

        private void SetPropertiesJson(JsonElement parsedJson)
        {
            if (parsedJson.ValueKind == JsonValueKind.Null)
            {
                return;
            }

            propertiesJson = parsedJson.ToString();

            if (parsedJson.TryGetProperty("webPartData", out JsonElement webPartData))
            {
                if (webPartData.TryGetProperty("properties", out JsonElement properties))
                {
                    Properties = properties;
                }

                if (webPartData.TryGetProperty("dataVersion", out JsonElement dataVersion))
                {
                    this.dataVersion = dataVersion.GetString().Trim('"');
                }

                if (webPartData.TryGetProperty("serverProcessedContent", out JsonElement serverProcessedContent))
                {
                    ServerProcessedContent = serverProcessedContent;
                }

                if (webPartData.TryGetProperty("dynamicDataPaths", out JsonElement dynamicDataPaths))
                {
                    DynamicDataPaths = dynamicDataPaths;
                }

                if (webPartData.TryGetProperty("dynamicDataValues", out JsonElement dynamicDataValues))
                {
                    DynamicDataValues = dynamicDataValues;
                }
            }
            else
            {
                if (parsedJson.TryGetProperty("properties", out JsonElement properties))
                {
                    Properties = properties;
                }
                else
                {
                    Properties = parsedJson;
                }

                if (parsedJson.TryGetProperty("dataVersion", out JsonElement dataVersion))
                {
                    this.dataVersion = dataVersion.GetString().Trim('"');
                }

                if (parsedJson.TryGetProperty("serverProcessedContent", out JsonElement serverProcessedContent))
                {
                    ServerProcessedContent = serverProcessedContent;
                }

                if (parsedJson.TryGetProperty("dynamicDataPaths", out JsonElement dynamicDataPaths))
                {
                    DynamicDataPaths = dynamicDataPaths;
                }

                if (parsedJson.TryGetProperty("dynamicDataValues", out JsonElement dynamicDataValues))
                {
                    DynamicDataValues = dynamicDataValues;
                }
            }
        }
        #endregion
    }
}
