using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace I0plus.XdUnityUI.Editor
{
    /// <summary>
    ///     ImageElement class.
    ///     based on Baum2.Editor.ImageElement class.
    /// </summary>
    public class ImageElement : Element
    {
        public readonly Dictionary<string, object> ComponentJson; // be parent component
        protected readonly Dictionary<string, object> ImageJson;

        public ImageElement(Dictionary<string, object> json, Element parent) : base(json, parent)
        {
            ComponentJson = json.GetDic("component");
            ImageJson = json.GetDic("image");
        }

        public override void Render(ref GameObject targetObject, RenderContext renderContext, GameObject parentObject)
        {
            GetOrCreateSelfObject(renderContext, ref targetObject, parentObject);

            var rect = targetObject.GetComponent<RectTransform>();

            if (parentObject)
                //親のパラメータがある場合､親にする 後のAnchor定義のため
                rect.SetParent(parentObject.transform);

            var image = targetObject.GetComponent<Image>();

            //if a image component is already present this means this go is part of a prefab and we skip the image setup since it has already been done
            //TODO: check if some parts still need to be done for prefabs that have local modifications
            if (image == null)
            {
                image = ElementUtil.GetOrAddComponent<Image>(targetObject);
                var sourceImage = ImageJson.Get("source_image");
                if (sourceImage != null)
                    image.sprite = renderContext.GetSprite(sourceImage);

                image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                var raycastTarget = ImageJson.GetBool("raycast_target");
                if (raycastTarget != null)
                    image.raycastTarget = raycastTarget.Value;

                image.type = Image.Type.Sliced;
                var imageType = ImageJson.Get("image_type");
                if (imageType != null)
                    switch (imageType.ToLower())
                    {
                        case "sliced":
                            image.type = Image.Type.Sliced;
                            break;
                        case "filled":
                            image.type = Image.Type.Filled;
                            break;
                        case "tiled":
                            image.type = Image.Type.Tiled;
                            break;
                        case "simple":
                            image.type = Image.Type.Simple;
                            break;
                        default:
                            Debug.LogAssertion("[XdUnityUI] unknown image_type:" + imageType);
                            break;
                    }

                var preserveAspect = ImageJson.GetBool("preserve_aspect");
                if (preserveAspect != null && preserveAspect.Value)
                {
                    // アスペクト比を保つ場合はSimpleにする
                    image.type = Image.Type.Simple;
                    image.preserveAspect = true;
                }
            }


            ElementUtil.SetupLayoutElement(targetObject, LayoutElementJson);
            ElementUtil.SetupRectTransform(targetObject, RectTransformJson);
        }
    }
}