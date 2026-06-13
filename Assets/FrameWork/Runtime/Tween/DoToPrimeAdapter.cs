using PrimeTween;
using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.DG.Tweening
{
    public enum UpdateType { Normal, Late, Fixed }
    public enum LoopType { Restart, Yoyo, Incremental, Rewind }
    public enum RotateMode
    {
        /// <summary> 360도 이상의 회전을 허용 X </summary>
        Fast,
        /// <summary> 360도 이상의 회전을 허용 O </summary>
        FastBeyond360,
    }
    public enum PathType { Linear, CatmullRom, CubicBezier }
    public enum PathMode { Ignore, Full3D, TopDown2D, Sidescroller2D }
    public enum TweenType
    {
        #region	Transform
        Move, MoveX, MoveY, MoveZ,
        LocalMove, LocalMoveX, LocalMoveY, LocalMoveZ,
        Rotate, LocalRotate, RotateQuaternion,
        Scale, ScaleX, ScaleY, ScaleZ,
        ShakePosition, ShakeRotation, ShakeScale,
        Jump, LocalJump,
        PunchPosition, PunchRotation, PunchScale,
        Path, LocalPath,
        #endregion

        #region RectTransform
        AnchorPos, AnchorPosX, AnchorPosY, SizeDelta,
        #endregion

        #region UI Components
        CanvasGroupFade,
        ImageFade, ImageColor, ImageFillAmount,
        TextFade, TextColor,
        #endregion

        #region World Space Renderers
        SpriteFade, SpriteColor,
        MaterialFade, MaterialColor,
        #endregion

        #region NGUI
        UIWidgetFade, UIWidgetColor,
        UIPanelFade,
        UILabelColor,
        #endregion

        #region Audio
        AudioVolume, AudioPitch,
        #endregion

        #region Camera
        CameraAspect, CameraBackgroundColor, CameraFarClipPlane,
        CameraFieldOfView, CameraNearClipPlane, CameraOrthoSize,
        CameraPixelRect, CameraRect
        #endregion
    }

    #region Custom Tween Data
    public struct ShakeData
    {
        public Vector3 strength;
        public int vibrato;
        public float randomness;
    }
    public struct JumpData
    {
        /// <summary> 목적지 좌표 </summary>
        public Vector3 endValue;
        /// <summary> 포물선 높이 </summary>
        public float jumpPower;
        /// <summary> 점프 횟수 </summary>
        public int numJumps;
    }
    public struct PathData
    {
        public Vector3[] waypoints;
        public PathType pathType;
        public PathMode pathMode;
        public int resolution;
    }

    public struct PunchData
    {
        public Vector3 punch;
        public int vibrato;
        public float elasticity;
        public bool snapping;
    }
    #endregion

    public struct DOTweenWrapper
    {
        public PrimeTween.Tween innerTween;
        public bool IsActive => innerTween.isAlive;
        private static readonly object dummyTarget = new object();

        public DOTweenWrapper(PrimeTween.Tween tween) { innerTween = tween; }

        public DOTweenWrapper OnComplete(Action callback)
        {
            innerTween.OnComplete(callback);
            return this;
        }

        public DOTweenWrapper OnUpdate(Action callback)
        {
            if (innerTween.isAlive)
            {
                innerTween.OnUpdate(dummyTarget, (target, tween) => callback?.Invoke());
            }
            return this;
        }

        public void Kill(bool complete = false)
        {
            if (complete) innerTween.Complete();
            innerTween.Stop();
        }
    }

    public struct TweenBuilder<T> where T : struct
    {
        public object target;
        public TweenType type;
        public TweenSettings settings;
        public bool isRelative;
        public RotateMode rotateMode;

        public T endValue;

        #region 세팅
        /// <summary>
        /// 애니메이션의 보간 방식을 설정
        /// </summary>
        public TweenBuilder<T> SetEase(Ease ease)
        {
            var next = this;
            next.settings.ease = ease;
            return next;
        }

        /// <summary>
        /// 무한 루프와 방식을 설정
        /// </summary>
        public TweenBuilder<T> SetLoop(bool isLoop, LoopType loopType = LoopType.Restart)
        {
            var next = this;
            next.settings.cycles = isLoop ? -1 : 1;
            next.settings.cycleMode = LoopToCycle(loopType);
            return next;
        }

        /// <summary>
        /// 반복 횟수와 방식을 설정
        /// </summary>
        public TweenBuilder<T> SetCycle(int count, LoopType loopType = LoopType.Restart)
        {
            var next = this;
            next.settings.cycles = count;
            next.settings.cycleMode = LoopToCycle(loopType);
            return next;
        }

        /// <summary>
        /// 시작 딜레이 부여
        /// </summary>
        public TweenBuilder<T> SetDelay(float delay)
        {
            var next = this;
            next.settings.startDelay = delay;
            return next;
        }

        /// <summary>
        /// true 일 경우 endValue가 현재 값에 더해지는 방식으로 트윈이 생성
        /// </summary>
        public TweenBuilder<T> SetRelative(bool isOn = true)
        {
            var next = this;
            next.isRelative = isOn;
            return next;
        }

        private CycleMode LoopToCycle(LoopType loopType)
        {
            return loopType switch
            {
                LoopType.Restart => CycleMode.Restart,
                LoopType.Yoyo => CycleMode.Yoyo,
                LoopType.Incremental => CycleMode.Incremental,
                LoopType.Rewind => CycleMode.Rewind,
                _ => CycleMode.Restart
            };
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static U Convert<U>(T value) where U : struct
        {
            return UnsafeUtility.As<T, U>(ref value);
        }

        public DOTweenWrapper Play()
        {
            PrimeTween.Tween tween = default;

            switch (type)
            {
                #region Transform

                #region Move (Position)
                case TweenType.Move:
                    {
                        var t = (Transform)target; var val = Convert<Vector3>(endValue);
                        tween = PrimeTween.Tween.Position(t, new TweenSettings<Vector3>(isRelative ? t.position + val : val, settings)); break;
                    }
                case TweenType.MoveX:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.PositionX(t, new TweenSettings<float>(isRelative ? t.position.x + val : val, settings)); break;
                    }
                case TweenType.MoveY:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.PositionY(t, new TweenSettings<float>(isRelative ? t.position.y + val : val, settings)); break;
                    }
                case TweenType.MoveZ:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.PositionZ(t, new TweenSettings<float>(isRelative ? t.position.z + val : val, settings)); break;
                    }

                case TweenType.LocalMove:
                    {
                        var t = (Transform)target; var val = Convert<Vector3>(endValue);
                        tween = PrimeTween.Tween.LocalPosition(t, new TweenSettings<Vector3>(isRelative ? t.localPosition + val : val, settings)); break;
                    }
                case TweenType.LocalMoveX:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.LocalPositionX(t, new TweenSettings<float>(isRelative ? t.localPosition.x + val : val, settings)); break;
                    }
                case TweenType.LocalMoveY:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.LocalPositionY(t, new TweenSettings<float>(isRelative ? t.localPosition.y + val : val, settings)); break;
                    }
                case TweenType.LocalMoveZ:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.LocalPositionZ(t, new TweenSettings<float>(isRelative ? t.localPosition.z + val : val, settings)); break;
                    }
                #endregion

                #region Rotate
                case TweenType.Rotate:
                    {
                        var t = (Transform)target; var val = Convert<Vector3>(endValue);
                        Vector3 startRot = t.eulerAngles;

                        // 축별 처리를 위해 값 보정 (Absolute일 때 0이 아닌 축만 반영)
                        Vector3 targetRot = isRelative
                            ? startRot + val
                            : new Vector3(val.x != 0 ? val.x : startRot.x, val.y != 0 ? val.y : startRot.y, val.z != 0 ? val.z : startRot.z);

                        if (rotateMode == RotateMode.FastBeyond360)
                        {
                            tween = PrimeTween.Tween.EulerAngles(t, new TweenSettings<Vector3>(startRot, targetRot, settings));
                        }
                        else
                        {
                            tween = PrimeTween.Tween.Rotation(t, new TweenSettings<Vector3>(targetRot, settings));
                        }
                        break;
                    }

                case TweenType.LocalRotate:
                    {
                        var t = (Transform)target; var val = Convert<Vector3>(endValue);
                        Vector3 startRot = t.localEulerAngles;

                        Vector3 targetRot = isRelative
                            ? startRot + val
                            : new Vector3(val.x != 0 ? val.x : startRot.x, val.y != 0 ? val.y : startRot.y, val.z != 0 ? val.z : startRot.z);

                        if (rotateMode == RotateMode.FastBeyond360)
                        {
                            tween = PrimeTween.Tween.LocalEulerAngles(t, new TweenSettings<Vector3>(startRot, targetRot, settings));
                        }
                        else
                        {
                            tween = PrimeTween.Tween.LocalRotation(t, new TweenSettings<Vector3>(targetRot, settings));
                        }
                        break;
                    }

                case TweenType.RotateQuaternion:
                    {
                        var t = (Transform)target; var val = Convert<Quaternion>(endValue);
                        var targetRot = isRelative ? t.rotation * val : val;
                        tween = PrimeTween.Tween.Rotation(t, new TweenSettings<Quaternion>(targetRot, settings));
                        break;
                    }
                #endregion

                #region Scale
                case TweenType.Scale:
                    {
                        var t = (Transform)target; var val = Convert<Vector3>(endValue);
                        tween = PrimeTween.Tween.Scale(t, new TweenSettings<Vector3>(isRelative ? t.localScale + val : val, settings)); break;
                    }
                case TweenType.ScaleX:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.ScaleX(t, new TweenSettings<float>(isRelative ? t.localScale.x + val : val, settings)); break;
                    }
                case TweenType.ScaleY:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.ScaleY(t, new TweenSettings<float>(isRelative ? t.localScale.y + val : val, settings)); break;
                    }
                case TweenType.ScaleZ:
                    {
                        var t = (Transform)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.ScaleZ(t, new TweenSettings<float>(isRelative ? t.localScale.z + val : val, settings)); break;
                    }
                #endregion

                #region Shake
                case TweenType.ShakePosition:
                    {
                        var val = Convert<ShakeData>(endValue);
                        tween = PrimeTween.Tween.ShakeLocalPosition((Transform)target, new ShakeSettings(val.strength, settings.duration, val.vibrato, easeBetweenShakes: settings.ease)); break;
                    }
                case TweenType.ShakeRotation:
                    {
                        var val = Convert<ShakeData>(endValue);
                        tween = PrimeTween.Tween.ShakeLocalRotation((Transform)target, new ShakeSettings(val.strength, settings.duration, val.vibrato, easeBetweenShakes: settings.ease)); break;
                    }
                case TweenType.ShakeScale:
                    {
                        var val = Convert<ShakeData>(endValue);
                        tween = PrimeTween.Tween.ShakeScale((Transform)target, new ShakeSettings(val.strength, settings.duration, val.vibrato, easeBetweenShakes: settings.ease)); break;
                    }
                #endregion

                #region Jump
                case TweenType.Jump:
                    {
                        var t = (Transform)target; var jd = Convert<JumpData>(endValue);

                        Vector3 startPos = t.position;
                        Vector3 finalTarget = isRelative ? startPos + jd.endValue : jd.endValue;

                        tween = PrimeTween.Tween.Custom(t, new TweenSettings<float>(0f, 1f, settings), (trans, progress) =>
                        {
                            if (trans == null) return;

                            Vector3 currentPos = Vector3.Lerp(startPos, finalTarget, progress);
                            float jumpProgress = (progress * jd.numJumps) % 1f;
                            float yOffset = 4f * jd.jumpPower * jumpProgress * (1f - jumpProgress);

                            currentPos.y += yOffset;
                            trans.position = currentPos;
                        });
                        break;
                    }

                case TweenType.LocalJump:
                    {
                        var t = (Transform)target; var jd = Convert<JumpData>(endValue);

                        Vector3 startPos = t.localPosition;
                        Vector3 finalTarget = isRelative ? startPos + jd.endValue : jd.endValue;

                        tween = PrimeTween.Tween.Custom(t, new TweenSettings<float>(0f, 1f, settings), (trans, progress) =>
                        {
                            if (trans == null) return;

                            Vector3 currentPos = Vector3.Lerp(startPos, finalTarget, progress);
                            float jumpProgress = (progress * jd.numJumps) % 1f;
                            float yOffset = 4f * jd.jumpPower * jumpProgress * (1f - jumpProgress);

                            currentPos.y += yOffset;
                            trans.localPosition = currentPos;
                        });
                        break;
                    }
                #endregion

                #region Punch
                case TweenType.PunchPosition:
                    {
                        var t = (Transform)target; var pd = Convert<PunchData>(endValue);

                        var punchSettings = new PrimeTween.ShakeSettings(strength: pd.punch, duration: settings.duration, frequency: pd.vibrato * 1.35f, enableFalloff: true, easeBetweenShakes: settings.ease, asymmetryFactor: Mathf.Clamp01(1f - pd.elasticity));
                        tween = PrimeTween.Tween.PunchLocalPosition(t, punchSettings);
                        break;
                    }

                case TweenType.PunchRotation:
                    {
                        var t = (Transform)target; var pd = Convert<PunchData>(endValue);

                        var punchSettings = new PrimeTween.ShakeSettings(strength: pd.punch, duration: settings.duration, frequency: pd.vibrato * 1.35f, enableFalloff: true, easeBetweenShakes: settings.ease, asymmetryFactor: Mathf.Clamp01(1f - pd.elasticity));
                        tween = PrimeTween.Tween.PunchLocalRotation(t, punchSettings);
                        break;
                    }

                case TweenType.PunchScale:
                    {
                        var t = (Transform)target; var pd = Convert<PunchData>(endValue);

                        var punchSettings = new PrimeTween.ShakeSettings(strength: pd.punch, duration: settings.duration, frequency: pd.vibrato * 1.35f, enableFalloff: true, easeBetweenShakes: settings.ease, asymmetryFactor: Mathf.Clamp01(1f - pd.elasticity));
                        tween = PrimeTween.Tween.PunchScale(t, punchSettings);
                        break;
                    }
                #endregion

                #region Path
                case TweenType.Path:
                    {
                        var t = (Transform)target; var pd = Convert<PathData>(endValue);
                        Vector3[] wp = pd.waypoints;
                        if (wp == null || wp.Length == 0) break;

                        Vector3 start = t.position;
                        Vector3[] fullPath = new Vector3[wp.Length + 1];
                        fullPath[0] = start;
                        for (int i = 0; i < wp.Length; i++) fullPath[i + 1] = isRelative ? start + wp[i] : wp[i];

                        tween = PrimeTween.Tween.Custom(t, new TweenSettings<float>(0f, 1f, settings), (trans, p) => trans.position = EvaluatePath(fullPath, p, pd.pathType));
                        break;
                    }
                case TweenType.LocalPath:
                    {
                        var t = (Transform)target; var pd = Convert<PathData>(endValue);
                        Vector3[] wp = pd.waypoints;
                        if (wp == null || wp.Length == 0) break;

                        Vector3 start = t.localPosition;
                        Vector3[] fullPath = new Vector3[wp.Length + 1];
                        fullPath[0] = start;
                        for (int i = 0; i < wp.Length; i++) fullPath[i + 1] = isRelative ? start + wp[i] : wp[i];

                        tween = PrimeTween.Tween.Custom(t, new TweenSettings<float>(0f, 1f, settings), (trans, p) => trans.localPosition = EvaluatePath(fullPath, p, pd.pathType));
                        break;
                    }
                #endregion

                #endregion

                #region RectTransform
                case TweenType.AnchorPos:
                    {
                        var rt = (RectTransform)target; var ev = Convert<Vector2>(endValue); var start = rt.anchoredPosition;
                        tween = PrimeTween.Tween.Custom(rt, new TweenSettings<Vector2>(start, isRelative ? start + ev : ev, settings), (t, v) => t.anchoredPosition = v); break;
                    }
                case TweenType.AnchorPosX:
                    {
                        var rt = (RectTransform)target; var ev = Convert<float>(endValue); var start = rt.anchoredPosition.x;
                        tween = PrimeTween.Tween.Custom(rt, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => { var p = t.anchoredPosition; p.x = v; t.anchoredPosition = p; }); break;
                    }
                case TweenType.AnchorPosY:
                    {
                        var rt = (RectTransform)target; var ev = Convert<float>(endValue); var start = rt.anchoredPosition.y;
                        tween = PrimeTween.Tween.Custom(rt, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => { var p = t.anchoredPosition; p.y = v; t.anchoredPosition = p; }); break;
                    }
                case TweenType.SizeDelta:
                    {
                        var rt = (RectTransform)target; var ev = Convert<Vector2>(endValue); var start = rt.sizeDelta;
                        tween = PrimeTween.Tween.Custom(rt, new TweenSettings<Vector2>(start, isRelative ? start + ev : ev, settings), (t, v) => t.sizeDelta = v); break;
                    }
                #endregion

                #region UI Components
                case TweenType.CanvasGroupFade:
                    {
                        var cg = (CanvasGroup)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.Alpha(cg, new TweenSettings<float>(isRelative ? cg.alpha + val : val, settings)); break;
                    }
                case TweenType.ImageColor:
                    {
                        var img = (Image)target; var val = Convert<Color>(endValue);
                        tween = PrimeTween.Tween.Color(img, new TweenSettings<Color>(isRelative ? img.color + val : val, settings)); break;
                    }
                case TweenType.ImageFade:
                    {
                        var img = (Image)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.Alpha(img, new TweenSettings<float>(isRelative ? img.color.a + val : val, settings)); break;
                    }
                case TweenType.ImageFillAmount:
                    {
                        var img = (Image)target; var ev = Convert<float>(endValue); var start = img.fillAmount;
                        tween = PrimeTween.Tween.Custom(img, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => t.fillAmount = v); break;
                    }

                case TweenType.TextColor:
                    {
                        var txt = (Text)target; var val = Convert<Color>(endValue);
                        tween = PrimeTween.Tween.Color(txt, new TweenSettings<Color>(isRelative ? txt.color + val : val, settings)); break;
                    }
                case TweenType.TextFade:
                    {
                        var txt = (Text)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.Alpha(txt, new TweenSettings<float>(isRelative ? txt.color.a + val : val, settings)); break;
                    }
                #endregion

                #region World Space Renderers
                case TweenType.SpriteColor:
                    {
                        var sr = (SpriteRenderer)target; var ev = Convert<Color>(endValue); var start = sr.color;
                        tween = PrimeTween.Tween.Custom(sr, new TweenSettings<Color>(start, isRelative ? start + ev : ev, settings), (t, v) => t.color = v); break;
                    }
                case TweenType.SpriteFade:
                    {
                        var sr = (SpriteRenderer)target; var ev = Convert<float>(endValue); var start = sr.color.a;
                        tween = PrimeTween.Tween.Custom(sr, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => { var c = t.color; c.a = v; t.color = c; }); break;
                    }

                case TweenType.MaterialColor:
                    {
                        var mat = (Material)target; var ev = Convert<Color>(endValue); var start = mat.color;
                        tween = PrimeTween.Tween.Custom(mat, new TweenSettings<Color>(start, isRelative ? start + ev : ev, settings), (t, v) => t.color = v); break;
                    }
                case TweenType.MaterialFade:
                    {
                        var mat = (Material)target; var ev = Convert<float>(endValue); var start = mat.color.a;
                        tween = PrimeTween.Tween.Custom(mat, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => { var c = t.color; c.a = v; t.color = c; }); break;
                    }
                #endregion

                #region NGUI Components
                //case TweenType.UIWidgetFade:
                //    {
                //        var nw = (UIWidget)target; var ev = Convert<float>(endValue); var start = nw.alpha;
                //        tween = PrimeTween.Tween.Custom(nw, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => t.alpha = v); break;
                //    }
                //case TweenType.UIWidgetColor:
                //    {
                //        var nw = (UIWidget)target; var ev = Convert<Color>(endValue); var start = nw.color;
                //        tween = PrimeTween.Tween.Custom(nw, new TweenSettings<Color>(start, isRelative ? start + ev : ev, settings), (t, v) => t.color = v); break;
                //    }
                //case TweenType.UIPanelFade:
                //    {
                //        var np = (UIPanel)target; var ev = Convert<float>(endValue); var start = np.alpha;
                //        tween = PrimeTween.Tween.Custom(np, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => t.alpha = v); break;
                //    }
                //case TweenType.UILabelColor:
                //    {
                //        var nl = (UILabel)target; var ev = Convert<Color>(endValue); var start = nl.color;
                //        tween = PrimeTween.Tween.Custom(nl, new TweenSettings<Color>(start, isRelative ? start + ev : ev, settings), (t, v) => t.color = v); break;
                //    }
                #endregion

                #region AudioSource
                case TweenType.AudioVolume:
                    {
                        var au = (AudioSource)target; var val = Convert<float>(endValue);
                        tween = PrimeTween.Tween.AudioVolume(au, new TweenSettings<float>(isRelative ? au.volume + val : val, settings)); break;
                    }
                case TweenType.AudioPitch:
                    {
                        var au = (AudioSource)target; var ev = Convert<float>(endValue); var start = au.pitch;
                        tween = PrimeTween.Tween.Custom(au, new TweenSettings<float>(start, isRelative ? start + ev : ev, settings), (t, v) => t.pitch = v); break;
                    }
                #endregion

                #region	Camera
                case TweenType.CameraAspect:
                    {
                        var cam = (Camera)target; var val = Convert<float>(endValue); var start = cam.aspect;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<float>(start, isRelative ? start + val : val, settings), (c, v) => c.aspect = v); break;
                    }
                case TweenType.CameraBackgroundColor:
                    {
                        var cam = (Camera)target; var val = Convert<Color>(endValue); var start = cam.backgroundColor;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<Color>(start, isRelative ? start + val : val, settings), (c, v) => c.backgroundColor = v); break;
                    }
                case TweenType.CameraFarClipPlane:
                    {
                        var cam = (Camera)target; var val = Convert<float>(endValue); var start = cam.farClipPlane;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<float>(start, isRelative ? start + val : val, settings), (c, v) => c.farClipPlane = v); break;
                    }
                case TweenType.CameraFieldOfView:
                    {
                        var cam = (Camera)target; var val = Convert<float>(endValue); var start = cam.fieldOfView;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<float>(start, isRelative ? start + val : val, settings), (c, v) => c.fieldOfView = v); break;
                    }
                case TweenType.CameraNearClipPlane:
                    {
                        var cam = (Camera)target; var val = Convert<float>(endValue); var start = cam.nearClipPlane;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<float>(start, isRelative ? start + val : val, settings), (c, v) => c.nearClipPlane = v); break;
                    }
                case TweenType.CameraOrthoSize:
                    {
                        var cam = (Camera)target; var val = Convert<float>(endValue); var start = cam.orthographicSize;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<float>(start, isRelative ? start + val : val, settings), (c, v) => c.orthographicSize = v); break;
                    }
                case TweenType.CameraPixelRect:
                    {
                        var cam = (Camera)target; var val = Convert<Rect>(endValue); var start = cam.pixelRect;
                        Rect targetRect = isRelative ? new Rect(start.x + val.x, start.y + val.y, start.width + val.width, start.height + val.height) : val;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<Rect>(start, targetRect, settings), (c, v) => c.pixelRect = v); break;
                    }
                case TweenType.CameraRect:
                    {
                        var cam = (Camera)target; var val = Convert<Rect>(endValue); var start = cam.rect;
                        Rect targetRect = isRelative ? new Rect(start.x + val.x, start.y + val.y, start.width + val.width, start.height + val.height) : val;
                        tween = PrimeTween.Tween.Custom(cam, new TweenSettings<Rect>(start, targetRect, settings), (c, v) => c.rect = v); break;
                    }
                    #endregion
            }

            return new DOTweenWrapper(tween);
        }

        private static Vector3 EvaluatePath(Vector3[] path, float t, PathType type)
        {
            if (path == null || path.Length == 0) return Vector3.zero;
            if (path.Length == 1) return path[0];
            if (t <= 0f) return path[0];
            if (t >= 1f) return path[path.Length - 1];

            if (type == PathType.Linear)
            {
                float p = t * (path.Length - 1);
                int i = (int)p;
                float frac = p - i;
                if (i >= path.Length - 1) return path[path.Length - 1];
                return Vector3.Lerp(path[i], path[i + 1], frac);
            }
            else
            {
                // Catmull-Rom 스플라인 보간 공식
                float p = t * (path.Length - 1);
                int i = (int)p;
                float frac = p - i;

                if (i >= path.Length - 1) return path[path.Length - 1];

                // 양 끝점의 곡선을 부드럽게 만들기 위해 가상의 가이드 포인트를 계산합니다.
                Vector3 p0 = i > 0 ? path[i - 1] : path[i] - (path[i + 1] - path[i]);
                Vector3 p1 = path[i];
                Vector3 p2 = path[i + 1];
                Vector3 p3 = i < path.Length - 2 ? path[i + 2] : p2 + (p2 - p1);

                float t2 = frac * frac;
                float t3 = t2 * frac;

                return 0.5f * (
                    (2f * p1) +
                    (-p0 + p2) * frac +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t3
                );
            }
        }
    }

    public class Sequence
    {
        public PrimeTween.Sequence innerSequence;
        public bool IsActive => innerSequence.isAlive;

        private PrimeTween.Tween updateTween;
        private static readonly object dummyTarget = new object();

        public static Sequence Create()
        {
            return new Sequence { innerSequence = PrimeTween.Sequence.Create() };
        }

        #region Append
        public Sequence Append<T>(TweenBuilder<T> builder) where T : struct
        {
            innerSequence.Chain(builder.Play().innerTween);
            return this;
        }
        public Sequence Append(DOTweenWrapper wrapper)
        {
            innerSequence.Chain(wrapper.innerTween);
            return this;
        }
        public Sequence Append(Sequence seq)
        {
            innerSequence.Chain(seq.innerSequence);
            return this;
        }
        #endregion

        #region Join
        public Sequence Join<T>(TweenBuilder<T> builder) where T : struct
        {
            innerSequence.Group(builder.Play().innerTween);
            return this;
        }
        public Sequence Join(DOTweenWrapper wrapper)
        {
            innerSequence.Group(wrapper.innerTween);
            return this;
        }
        public Sequence Join(Sequence seq)
        {
            innerSequence.Group(seq.innerSequence);
            return this;
        }
        #endregion

        #region Insert
        public Sequence Insert<T>(float atPosition, TweenBuilder<T> builder) where T : struct
        {
            innerSequence.Insert(atPosition, builder.Play().innerTween);
            return this;
        }
        public Sequence Insert(float atPosition, DOTweenWrapper wrapper)
        {
            innerSequence.Insert(atPosition, wrapper.innerTween);
            return this;
        }
        public Sequence Insert(float atPosition, Sequence seq)
        {
            innerSequence.Insert(atPosition, seq.innerSequence);
            return this;
        }
        #endregion

        #region Prepend
        public Sequence Prepend<T>(TweenBuilder<T> builder) where T : struct
        {
            var newSeq = PrimeTween.Sequence.Create();
            newSeq.Chain(builder.Play().innerTween);
            newSeq.Chain(innerSequence);
            innerSequence = newSeq;
            return this;
        }
        public Sequence Prepend(DOTweenWrapper wrapper)
        {
            var newSeq = PrimeTween.Sequence.Create();
            newSeq.Chain(wrapper.innerTween);
            newSeq.Chain(innerSequence);
            innerSequence = newSeq;
            return this;
        }
        public Sequence Prepend(Sequence seq)
        {
            var newSeq = PrimeTween.Sequence.Create();
            newSeq.Chain(seq.innerSequence);
            newSeq.Chain(innerSequence);
            innerSequence = newSeq;
            return this;
        }
        #endregion

        #region Intervals 
        public Sequence AppendInterval(float interval)
        {
            innerSequence.ChainDelay(interval);
            return this;
        }
        public Sequence PrependInterval(float interval)
        {
            var newSeq = PrimeTween.Sequence.Create();
            newSeq.ChainDelay(interval);
            newSeq.Chain(innerSequence);
            innerSequence = newSeq;
            return this;
        }
        #endregion

        #region Callbacks 
        public Sequence AppendCallback(Action callback)
        {
            innerSequence.ChainCallback(callback);
            return this;
        }
        public Sequence InsertCallback(float atPosition, Action callback)
        {
            innerSequence.InsertCallback(atPosition, callback);
            return this;
        }
        public Sequence PrependCallback(Action callback)
        {
            var newSeq = PrimeTween.Sequence.Create();
            newSeq.ChainCallback(callback);
            newSeq.Chain(innerSequence);
            innerSequence = newSeq;
            return this;
        }
        #endregion

        #region Lifecycle 
        public Sequence OnComplete(Action callback)
        {
            innerSequence.OnComplete(callback);
            return this;
        }
        public Sequence OnStart(Action callback)
        {
            innerSequence.InsertCallback(0f, callback);
            return this;
        }
        public Sequence OnUpdate(Action callback)
        {
            // PrimeTween의 Sequence는 기본적으로 OnUpdate 콜백을 갖고 있지 않으므로
            // Sequence 생명주기 동안 동작하는 무한 Tween을 임시로 생성하여 처리합니다.
            updateTween = PrimeTween.Tween.Custom(dummyTarget, 0f, 1f, 100000f, (t, v) => callback?.Invoke());
            innerSequence.OnComplete(() => updateTween.Stop());
            return this;
        }
        #endregion

        public void Kill(bool complete = false)
        {
            if (updateTween.isAlive) updateTween.Stop();
            if (complete) innerSequence.Complete();
            innerSequence.Stop();
        }
    }

    public static class DOTween
    {
        public static Sequence Sequence()
        {
            return DG.Tweening.Sequence.Create();
        }
    }

    public static class DoToPrimeExtensions
    {
        private static TweenSettings Set(float duration) => new TweenSettings(duration, ease: PrimeTween.Ease.Default);

        #region Transform

        #region Move (Position)
        public static TweenBuilder<Vector3> DOMove(this Transform t, Vector3 endValue, float duration)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Move, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOMoveX(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.MoveX, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOMoveY(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.MoveY, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOMoveZ(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.MoveZ, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Vector3> DOLocalMove(this Transform t, Vector3 endValue, float duration)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.LocalMove, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOLocalMoveX(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.LocalMoveX, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOLocalMoveY(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.LocalMoveY, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOLocalMoveZ(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.LocalMoveZ, endValue = endValue, settings = Set(duration) };
        #endregion

        #region Rotate
        public static TweenBuilder<Vector3> DORotate(this Transform t, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Rotate, endValue = endValue, settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DORotateX(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Rotate, endValue = new Vector3(endValue, 0, 0), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DORotateY(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Rotate, endValue = new Vector3(0, endValue, 0), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DORotateZ(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Rotate, endValue = new Vector3(0, 0, endValue), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DOLocalRotate(this Transform t, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.LocalRotate, endValue = endValue, settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DOLocalRotateX(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.LocalRotate, endValue = new Vector3(endValue, 0, 0), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DOLocalRotateY(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.LocalRotate, endValue = new Vector3(0, endValue, 0), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Vector3> DOLocalRotateZ(this Transform t, float endValue, float duration, RotateMode mode = RotateMode.Fast)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.LocalRotate, endValue = new Vector3(0, 0, endValue), settings = Set(duration), rotateMode = mode };
        public static TweenBuilder<Quaternion> DORotateQuaternion(this Transform t, Quaternion endValue, float duration)
            => new TweenBuilder<Quaternion> { target = t, type = TweenType.RotateQuaternion, endValue = endValue, settings = Set(duration) };
        #endregion

        #region Scale
        public static TweenBuilder<Vector3> DOScale(this Transform t, Vector3 endValue, float duration)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Scale, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Vector3> DOScale(this Transform t, float endValue, float duration)
            => new TweenBuilder<Vector3> { target = t, type = TweenType.Scale, endValue = new Vector3(endValue, endValue, endValue), settings = Set(duration) };
        public static TweenBuilder<float> DOScaleX(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.ScaleX, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOScaleY(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.ScaleY, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOScaleZ(this Transform t, float endValue, float duration)
            => new TweenBuilder<float> { target = t, type = TweenType.ScaleZ, endValue = endValue, settings = Set(duration) };
        #endregion

        #region Shake
        public static TweenBuilder<ShakeData> DOShakePosition(this Transform t, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f)
            => new TweenBuilder<ShakeData> { target = t, type = TweenType.ShakePosition, endValue = new ShakeData { strength = Vector3.one * strength, vibrato = vibrato, randomness = randomness }, settings = Set(duration) };
        public static TweenBuilder<ShakeData> DOShakeRotation(this Transform t, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f)
            => new TweenBuilder<ShakeData> { target = t, type = TweenType.ShakeRotation, endValue = new ShakeData { strength = Vector3.one * strength, vibrato = vibrato, randomness = randomness }, settings = Set(duration) };
        public static TweenBuilder<ShakeData> DOShakePosition(this Transform t, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f)
            => new TweenBuilder<ShakeData> { target = t, type = TweenType.ShakePosition, endValue = new ShakeData { strength = strength, vibrato = vibrato, randomness = randomness }, settings = Set(duration) };
        public static TweenBuilder<ShakeData> DOShakeRotation(this Transform t, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f)
            => new TweenBuilder<ShakeData> { target = t, type = TweenType.ShakeRotation, endValue = new ShakeData { strength = strength, vibrato = vibrato, randomness = randomness }, settings = Set(duration) };
        public static TweenBuilder<ShakeData> DOShakeScale(this Transform t, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f)
            => new TweenBuilder<ShakeData> { target = t, type = TweenType.ShakeScale, endValue = new ShakeData { strength = Vector3.one * strength, vibrato = vibrato, randomness = randomness }, settings = Set(duration) };
        #endregion

        #region Jump
        public static TweenBuilder<JumpData> DOJump(this Transform t, Vector3 endValue, float jumpPower, int numJumps, float duration)
            => new TweenBuilder<JumpData> { target = t, type = TweenType.Jump, endValue = new JumpData { endValue = endValue, jumpPower = jumpPower, numJumps = numJumps }, settings = Set(duration) };
        public static TweenBuilder<JumpData> DOLocalJump(this Transform t, Vector3 endValue, float jumpPower, int numJumps, float duration)
            => new TweenBuilder<JumpData> { target = t, type = TweenType.LocalJump, endValue = new JumpData { endValue = endValue, jumpPower = jumpPower, numJumps = numJumps }, settings = Set(duration) };
        #endregion

        #region Punch
        public static TweenBuilder<PunchData> DOPunchPosition(this Transform t, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f, bool snapping = false)
            => new TweenBuilder<PunchData> { target = t, type = TweenType.PunchPosition, endValue = new PunchData { punch = punch, vibrato = vibrato, elasticity = elasticity, snapping = snapping }, settings = Set(duration) };

        public static TweenBuilder<PunchData> DOPunchRotation(this Transform t, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
            => new TweenBuilder<PunchData> { target = t, type = TweenType.PunchRotation, endValue = new PunchData { punch = punch, vibrato = vibrato, elasticity = elasticity }, settings = Set(duration) };

        public static TweenBuilder<PunchData> DOPunchScale(this Transform t, Vector3 punch, float duration, int vibrato = 10, float elasticity = 1f)
            => new TweenBuilder<PunchData> { target = t, type = TweenType.PunchScale, endValue = new PunchData { punch = punch, vibrato = vibrato, elasticity = elasticity }, settings = Set(duration) };
        #endregion

        #region Path
        public static TweenBuilder<PathData> DOPath(this Transform t, Vector3[] waypoints, float duration, PathType pathType = PathType.Linear, PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null)
            => new TweenBuilder<PathData> { target = t, type = TweenType.Path, endValue = new PathData { waypoints = waypoints, pathType = pathType, pathMode = pathMode, resolution = resolution }, settings = Set(duration) };

        public static TweenBuilder<PathData> DOLocalPath(this Transform t, Vector3[] waypoints, float duration, PathType pathType = PathType.Linear, PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null)
            => new TweenBuilder<PathData> { target = t, type = TweenType.LocalPath, endValue = new PathData { waypoints = waypoints, pathType = pathType, pathMode = pathMode, resolution = resolution }, settings = Set(duration) };
        #endregion

        #endregion

        #region RectTransform
        public static TweenBuilder<Vector2> DOAnchorPos(this RectTransform rt, Vector2 endValue, float duration)
            => new TweenBuilder<Vector2> { target = rt, type = TweenType.AnchorPos, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOAnchorPosX(this RectTransform rt, float endValue, float duration)
            => new TweenBuilder<float> { target = rt, type = TweenType.AnchorPosX, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOAnchorPosY(this RectTransform rt, float endValue, float duration)
            => new TweenBuilder<float> { target = rt, type = TweenType.AnchorPosY, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Vector2> DOSizeDelta(this RectTransform rt, Vector2 endValue, float duration)
            => new TweenBuilder<Vector2> { target = rt, type = TweenType.SizeDelta, endValue = endValue, settings = Set(duration) };
        #endregion

        #region UI Components
        public static TweenBuilder<float> DOFade(this CanvasGroup cg, float endValue, float duration)
            => new TweenBuilder<float> { target = cg, type = TweenType.CanvasGroupFade, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Color> DOColor(this Image img, Color endValue, float duration)
            => new TweenBuilder<Color> { target = img, type = TweenType.ImageColor, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFade(this Image img, float endValue, float duration)
            => new TweenBuilder<float> { target = img, type = TweenType.ImageFade, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFillAmount(this Image img, float endValue, float duration)
            => new TweenBuilder<float> { target = img, type = TweenType.ImageFillAmount, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Color> DOColor(this Text txt, Color endValue, float duration)
            => new TweenBuilder<Color> { target = txt, type = TweenType.TextColor, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFade(this Text txt, float endValue, float duration)
            => new TweenBuilder<float> { target = txt, type = TweenType.TextFade, endValue = endValue, settings = Set(duration) };
        #endregion

        #region World Space Renderers
        public static TweenBuilder<Color> DOColor(this SpriteRenderer sr, Color endValue, float duration)
            => new TweenBuilder<Color> { target = sr, type = TweenType.SpriteColor, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFade(this SpriteRenderer sr, float endValue, float duration)
            => new TweenBuilder<float> { target = sr, type = TweenType.SpriteFade, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Color> DOColor(this Material mat, Color endValue, float duration)
            => new TweenBuilder<Color> { target = mat, type = TweenType.MaterialColor, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFade(this Material mat, float endValue, float duration)
            => new TweenBuilder<float> { target = mat, type = TweenType.MaterialFade, endValue = endValue, settings = Set(duration) };
        #endregion

        #region NGUI Components
        //public static TweenBuilder<float> DOFade(this UIWidget widget, float endValue, float duration)
        //    => new TweenBuilder<float> { target = widget, type = TweenType.UIWidgetFade, endValue = endValue, settings = Set(duration) };
        //public static TweenBuilder<Color> DOColor(this UIWidget widget, Color endValue, float duration)
        //    => new TweenBuilder<Color> { target = widget, type = TweenType.UIWidgetColor, endValue = endValue, settings = Set(duration) };
        //public static TweenBuilder<float> DOFade(this UIPanel panel, float endValue, float duration)
        //    => new TweenBuilder<float> { target = panel, type = TweenType.UIPanelFade, endValue = endValue, settings = Set(duration) };
        //public static TweenBuilder<Color> DOColor(this UILabel label, Color endValue, float duration)
        //    => new TweenBuilder<Color> { target = label, type = TweenType.UILabelColor, endValue = endValue, settings = Set(duration) };
        #endregion

        #region AudioSource
        public static TweenBuilder<float> DOFade(this AudioSource audio, float endValue, float duration)
            => new TweenBuilder<float> { target = audio, type = TweenType.AudioVolume, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOPitch(this AudioSource audio, float endValue, float duration)
            => new TweenBuilder<float> { target = audio, type = TweenType.AudioPitch, endValue = endValue, settings = Set(duration) };
        #endregion

        #region Camera
        public static TweenBuilder<float> DOAspect(this Camera cam, float endValue, float duration)
            => new TweenBuilder<float> { target = cam, type = TweenType.CameraAspect, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Color> DOColor(this Camera cam, Color endValue, float duration)
            => new TweenBuilder<Color> { target = cam, type = TweenType.CameraBackgroundColor, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFarClipPlane(this Camera cam, float endValue, float duration)
            => new TweenBuilder<float> { target = cam, type = TweenType.CameraFarClipPlane, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOFieldOfView(this Camera cam, float endValue, float duration)
            => new TweenBuilder<float> { target = cam, type = TweenType.CameraFieldOfView, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DONearClipPlane(this Camera cam, float endValue, float duration)
            => new TweenBuilder<float> { target = cam, type = TweenType.CameraNearClipPlane, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<float> DOOrthoSize(this Camera cam, float endValue, float duration)
            => new TweenBuilder<float> { target = cam, type = TweenType.CameraOrthoSize, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Rect> DOPixelRect(this Camera cam, Rect endValue, float duration)
            => new TweenBuilder<Rect> { target = cam, type = TweenType.CameraPixelRect, endValue = endValue, settings = Set(duration) };
        public static TweenBuilder<Rect> DORect(this Camera cam, Rect endValue, float duration)
            => new TweenBuilder<Rect> { target = cam, type = TweenType.CameraRect, endValue = endValue, settings = Set(duration) };

        #region Camera Shake
        public static TweenBuilder<ShakeData> DOShakePosition(this Camera cam, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f)
            => cam.transform.DOShakePosition(duration, Vector3.one * strength, vibrato, randomness);
        public static TweenBuilder<ShakeData> DOShakeRotation(this Camera cam, float duration, float strength = 90f, int vibrato = 10, float randomness = 90f)
            => cam.transform.DOShakeRotation(duration, Vector3.one * strength, vibrato, randomness);
        public static TweenBuilder<ShakeData> DOShakePosition(this Camera cam, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f)
            => cam.transform.DOShakePosition(duration, strength, vibrato, randomness);
        public static TweenBuilder<ShakeData> DOShakeRotation(this Camera cam, float duration, Vector3 strength, int vibrato = 10, float randomness = 90f)
            => cam.transform.DOShakeRotation(duration, strength, vibrato, randomness);
        #endregion
        #endregion

        #region Kill
        public static void DOKill(this Transform t, bool complete = false) => PrimeTween.Tween.StopAll(t);
        public static void DOKill(this GameObject go, bool complete = false) => PrimeTween.Tween.StopAll(go.transform);
        #endregion
    }
}