using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XFFlipViewNative;
using XFFlipViewNative.Droid;
using View = Xamarin.Forms.View;

[assembly:ExportRenderer(typeof(FilppableContentViewXF), typeof(FilppableContentViewXFRenderer))]
namespace XFFlipViewNative.Droid
{
    public class FilppableContentViewXFRenderer : ViewRenderer
    {
        private float _cameraDistance;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (((FilppableContentViewXF) e.NewElement) != null)
            {
                // Calculating Camera Distance to be used at Animation Runtime
                // https://forums.xamarin.com/discussion/49978/changing-default-perspective-after-rotation
                var distance = 8000;
                _cameraDistance = Context.Resources.DisplayMetrics.Density*distance;
                

                ((FilppableContentViewXF) e.NewElement).FlipThisShyiatFrontToBack 
                    = AndroidNativeFlipFrontToBack(((FilppableContentViewXF)e.NewElement));
                
                ((FilppableContentViewXF) e.NewElement).FlipThisShyiatBackToFront 
                    = AndroidNativeFlipBackToFront(((FilppableContentViewXF)e.NewElement));
            }
        }

        private Command AndroidNativeFlipFrontToBack(FilppableContentViewXF newElementInstance)
        {
            return new Command(() =>
            {
                AnimateFlipHorizontally(newElementInstance, true);
            });
        }

        private Command AndroidNativeFlipBackToFront(FilppableContentViewXF newElementInstance)
        {
            return new Command(() =>
            {
                this.RotationY = 0;

                AnimateFlipHorizontally(newElementInstance, false);
            });
        }

        private void AnimateFlipHorizontally(FilppableContentViewXF newElementInstance, bool isFrontToBack)
        {
            SetCameraDistance(_cameraDistance);

            ObjectAnimator animateYAxis0To90 = ObjectAnimator.OfFloat(this, "RotationY", 0.0f, -90f);
            animateYAxis0To90.SetDuration(500);

            animateYAxis0To90.Start();
            animateYAxis0To90.Update += (sender, args) =>
            {
                // On every animation Frame we have to update the Camera Distance since Xamarin overrides it somewhere
                SetCameraDistance(_cameraDistance);
            };

            animateYAxis0To90.AnimationEnd += (sender, args) =>
            {
                if (isFrontToBack)
                {
                    newElementInstance.SwitchViewsFlipFromFrontToBack();
                }
                else
                {
                    newElementInstance.SwitchViewsFlipFromBackToFront();
                }

                this.RotationY = -270;

                ObjectAnimator animateYAxis90To180 = ObjectAnimator.OfFloat(this, "RotationY", -270f, -360f);
                animateYAxis90To180.SetDuration(500);
                animateYAxis90To180.Start();
                animateYAxis90To180.Update += (sender1, args1) =>
                {
                    // On every animation Frame we have to update the Camera Distance since Xamarin overrides it somewhere
                    SetCameraDistance(_cameraDistance);
                };
            };
        }
    }
}