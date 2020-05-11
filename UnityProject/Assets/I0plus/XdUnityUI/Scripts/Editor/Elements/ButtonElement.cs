using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XdUnityUI.Editor
{
    /// <summary>
    /// ButtonElement class.
    /// based on Baum2.Editor.ButtonElement class.
    /// </summary>
    public class ButtonElement : GroupElement
    {
        protected readonly Dictionary<string, object> ButtonJson;

        public ButtonElement(Dictionary<string, object> json, Element parent) : base(json, parent)
        {
            ButtonJson = json.GetDic("button");
        }

        public override GameObject Render(RenderContext renderContext, GameObject parentObject)
        {
            var go = CreateSelf(renderContext);
            var rect = go.GetComponent<RectTransform>();
            if (parentObject)
            {
                //親のパラメータがある場合､親にする 後のAnchor定義のため
                rect.SetParent(parentObject.transform);
            }

            var children = RenderChildren(renderContext, go);

            var button = go.AddComponent<Button>();
            GameObject targetImageObject = null;
            var deleteObjects = new Dictionary<GameObject, bool>();
            if (ButtonJson != null)
            {
                var type = ButtonJson.Get("transition");
                switch (type)
                {
                    case "sprite-swap":
                        button.transition = Selectable.Transition.SpriteSwap;
                        break;
                    case "color-tint":
                        button.transition = Selectable.Transition.ColorTint;
                        break;
                    case "animation":
                        button.transition = Selectable.Transition.Animation;
                        break;
                    case "none":
                        button.transition = Selectable.Transition.None;
                        break;
                    default:
                        button.transition = Selectable.Transition.ColorTint;
                        break;
                }

                var targetImage =
                    ElementUtil.FindComponentByClassName<Image>(children, ButtonJson.Get("target_graphic_class"));
                if (targetImage != null)
                {
                    button.targetGraphic = targetImage;
                    targetImageObject = targetImage.gameObject;
                }

                var spriteStateJson = ButtonJson.GetDic("sprite_state");
                if (spriteStateJson != null)
                {
                    var spriteState = new SpriteState();
                    var image = ElementUtil.FindComponentByClassName<Image>(children,
                        spriteStateJson.Get("highlighted_sprite_class"));
                    if (image != null)
                    {
                        spriteState.highlightedSprite = image.sprite;
                        deleteObjects[image.gameObject] = true;
                    }

                    image = ElementUtil.FindComponentByClassName<Image>(children,
                        spriteStateJson.Get("pressed_sprite_class"));
                    if (image != null)
                    {
                        spriteState.pressedSprite = image.sprite;
                        deleteObjects[image.gameObject] = true;
                    }

#if UNITY_2019_1_OR_NEWER
                    image = ElementUtil.FindComponentByClassName<Image>(children,
                        spriteStateJson.Get("selected_sprite_class"));
                    if (image != null)
                    {
                        spriteState.selectedSprite = image.sprite;
                        deleteObjects[image.gameObject] = true;
                    }
#endif

                    image = ElementUtil.FindComponentByClassName<Image>(children,
                        spriteStateJson.Get("disabled_sprite_class"));
                    if (image != null)
                    {
                        spriteState.disabledSprite = image.sprite;
                        deleteObjects[image.gameObject] = true;
                    }

                    button.spriteState = spriteState;
                }
            }

            foreach (var keyValuePair in deleteObjects)
            {
                if (keyValuePair.Key != targetImageObject)
                    Object.DestroyImmediate(keyValuePair.Key);
            }

            // TargetGraphicが設定されなかった場合
            if (button.targetGraphic == null)
            {
                // 子供からImage持ちを探す

                var image = go.GetComponentInChildren<Image>();
                if (image == null)
                {
                    // componentでないか探す
                    image = go.GetComponent<Image>();
                }

                button.targetGraphic = image;
            }

            ElementUtil.SetupRectTransform(go, RectTransformJson);
            ElementUtil.SetupLayoutElement(go, LayoutElementJson);
            ElementUtil.SetupComponents(go, ComponentsJson);
            return go;
        }
    }
}