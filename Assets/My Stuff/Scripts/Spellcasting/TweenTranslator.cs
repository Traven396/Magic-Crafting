namespace AgeOfEnlightenment.Spellcasting
{
    using DG.Tweening;
    using UnityEngine;

    public static class TweenTranslator
    {

        public static Tweener CreateTweener(RouteTweenSettings settings, SpellActionContext context)
        {
            Tweener createdTween = null;

            //We check and see if there is an actual runtime object with the desired tag
            if (!context.TryGetRuntimeObject(settings.TargetObjectKey, out GameObject targetGameObject))
            {
                Debug.LogError("Cannot find that object in runtime");
                return null;
            }

            switch (settings.Value)
            {
                case RouteTweenValue.Scale:
                    createdTween = CreateScaleTweener(settings, targetGameObject);
                    break;
                case RouteTweenValue.Local_Position:

                    break;
                case RouteTweenValue.World_Position:

                    break;
                case RouteTweenValue.Rotation:
                    createdTween = CreateRotationTweener(settings, targetGameObject);
                    break;
            }

            return createdTween;
        }

        static Tweener CreateScaleTweener(RouteTweenSettings settings, GameObject targetGameObject)
        {
            Tweener scaleTweener = null;

            Vector3 modifier = settings.SeperateAxis ? settings.ChangeVector : Vector3.one * settings.ChangeValue;

            switch (settings.Action)
            {
                case RouteTweenAction.Add:
                    scaleTweener = targetGameObject.transform.DOScale(targetGameObject.transform.localScale + modifier, settings.Duration);

                    break;
                case RouteTweenAction.Subtract:
                    scaleTweener = targetGameObject.transform.DOScale(targetGameObject.transform.localScale - modifier, settings.Duration);

                    break;
                case RouteTweenAction.To:
                    scaleTweener = targetGameObject.transform.DOScale(modifier, settings.Duration);

                    break;
                case RouteTweenAction.From:
                    scaleTweener = targetGameObject.transform.DOScale(modifier, settings.Duration).From();

                    break;
                case RouteTweenAction.Punch:
                    scaleTweener = targetGameObject.transform.DOPunchScale(modifier, settings.Duration);

                    break;
                case RouteTweenAction.Shake:
                    scaleTweener = targetGameObject.transform.DOShakeScale(settings.Duration, modifier);

                    break;
            }

            if (settings.Loop)
            {
                scaleTweener.SetLoops(-1, settings.LoopType);
            }


            return scaleTweener;
        }

        static Tweener CreateRotationTweener(RouteTweenSettings settings, GameObject targetGameObject)
        {
            Tweener rotateTweener = null;

            Vector3 modifier = settings.SeperateAxis ? settings.ChangeVector : Vector3.one * settings.ChangeValue;

            switch (settings.Action)
            {
                case RouteTweenAction.Add:
                    rotateTweener = targetGameObject.transform.DORotate(targetGameObject.transform.rotation.eulerAngles + modifier, settings.Duration);

                    break;
                case RouteTweenAction.Subtract:
                    rotateTweener = targetGameObject.transform.DORotate(targetGameObject.transform.rotation.eulerAngles - modifier, settings.Duration);

                    break;
                case RouteTweenAction.To:
                    rotateTweener = targetGameObject.transform.DORotate(modifier, settings.Duration);

                    break;
                case RouteTweenAction.From:
                    rotateTweener = targetGameObject.transform.DORotate(modifier, settings.Duration).From();

                    break;
                case RouteTweenAction.Punch:
                    rotateTweener = targetGameObject.transform.DOPunchRotation(modifier, settings.Duration);

                    break;
                case RouteTweenAction.Shake:
                    rotateTweener = targetGameObject.transform.DOShakeRotation(settings.Duration, modifier);

                    break;
            }

            if (settings.Loop)
            {
                rotateTweener.SetLoops(-1, settings.LoopType);
            }


            return rotateTweener;
        }

        static Tweener CreatePositionTweener(RouteTweenSettings settings, GameObject targetGameObject)
        {
            Tweener scaleTweener = null;

            Vector3 modifier = settings.SeperateAxis ? settings.ChangeVector : Vector3.one * settings.ChangeValue;

            switch (settings.Action)
            {
                case RouteTweenAction.Add:
                    scaleTweener = targetGameObject.transform.DOScale(targetGameObject.transform.localScale + modifier, settings.Duration);

                    break;
                case RouteTweenAction.Subtract:
                    scaleTweener = targetGameObject.transform.DOScale(targetGameObject.transform.localScale - modifier, settings.Duration);

                    break;
                case RouteTweenAction.To:
                    scaleTweener = targetGameObject.transform.DOScale(modifier, settings.Duration);

                    break;
                case RouteTweenAction.From:
                    scaleTweener = targetGameObject.transform.DOScale(modifier, settings.Duration).From();

                    break;
                case RouteTweenAction.Punch:
                    scaleTweener = targetGameObject.transform.DOPunchScale(modifier, settings.Duration);

                    break;
                case RouteTweenAction.Shake:
                    scaleTweener = targetGameObject.transform.DOShakeScale(settings.Duration, modifier);

                    break;
            }

            if (settings.Loop)
            {
                scaleTweener.SetLoops(-1, settings.LoopType);
            }


            return scaleTweener;
        }
    }
}