using UnityEngine;
using System.Collections;
using Skill.Framework.UI;
using System.Collections.Generic;
using System;

namespace Skill.Editor.Curve
{
    public class FunctionTrack : CanvasPanel
    {
        protected class CurvePart
        {
            public int StartIndex;
            public int Count;
        }
        
        public Func<float, float> Func{ get; protected set; }
        public TimeLineCurveView View { get; protected set; }
        public Color Color { get; set; }
        
        protected List<Vector2> _Samples;
        protected List<CurvePart> _Parts;
        protected bool _Resample;
        protected float _MinValue;
        protected float _MaxValue;
        
        public override void Invalidate()
        {
            _Resample = true;
            base.Invalidate();
        }
        protected override void OnLayoutChanged()
        {
            _Resample = true;
            base.OnLayoutChanged();
        }

        public FunctionTrack(TimeLineCurveView view, Func<float, float> func)
        {
            if (view == null) throw new System.ArgumentNullException("Invalid TimeLineCurveView");
            if (func == null) throw new System.ArgumentNullException("Invalid Func");

            this.View = view;
            this.Func = func;
            this.Color = Color.green;
            
            this._Samples = new List<Vector2>(2000);
            this._Parts = new List<CurvePart>(5);
            this.IsInScrollView = true;
        }
        
        protected CurvePart CreatePart(int startIndex)
        {
            CurvePart part = new CurvePart();
            part.StartIndex = startIndex;
            part.Count = 0;
            _Parts.Add(part);
            return part;
        }

        protected void Resample()
        {
            if (_Resample)
            {
                _Resample = false;
                _Parts.Clear();
                if (Func != null)
                {
                    float minTime, maxTime;

                    int index = -1;
                    CurvePart part = null;

                    double startTime = View.TimeLine.StartVisible;

                    double endTime = View.TimeLine.EndVisible;

                    double stepTime = (endTime - startTime) / View.RenderArea.width;

                    if (stepTime <= 0.0)
                    {
                        return;
                    }

                    double time = startTime;
                    while (time <= endTime)
                    {
                        float fTime = (float)time;
                        float value = Func(fTime);

                        if (value >= View.MinVisibleValue && value <= View.MaxVisibleValue)
                        {
                            index++;
                            Vector2 sample = GetPoint(fTime, value, false);
                            if (part == null)
                                part = CreatePart(index);

                            if (_Samples.Count > index)
                                _Samples[index] = sample;
                            else
                                _Samples.Add(sample);

                            part.Count++;
                        }
                        else if (part != null)
                        {
                            part = null;
                            index++;
                            if (_Samples.Count > index)
                                _Samples[index] = _Samples[index - 1];
                            else
                                _Samples.Add(_Samples[index - 1]);
                        }

                        time += stepTime;
                    }
                }
            }
        }
        
        protected override void Render()
        {
            Resample();
            if (_Parts.Count > 0)
            {
                foreach (var p in _Parts)
                {
                    if (p.Count > 1)
                        Skill.Editor.LineDrawer.DrawPolyLine(_Samples, Color, p.StartIndex, p.Count);
                }
            }
            base.Render();
        }
        
        public void CurveChange(object sender, System.EventArgs e)
        {
            _Resample = true;
        }
        
        public Vector2 GetPoint(float time, float value, bool relative)
        {
            Rect ra = RenderArea;
            return GetPoint(time, value, relative, ref ra);
        }
        
        protected Vector2 GetPoint(float time, float value, bool relative, ref Rect renderArea)
        {
            return new Vector2(GetX(time, relative, ref renderArea), GetY(value, relative, ref renderArea));
        }
        
        public float GetX(float time, bool relative)
        {
            Rect ra = RenderArea;
            return GetX(time, relative, ref ra);
        }
        protected float GetX(float time, bool relative, ref Rect renderArea)
        {
            float x;
            if (renderArea.width > Mathf.Epsilon)
                x = renderArea.x + (float)((time - View.TimeLine.MinTime) / (View.TimeLine.MaxTime - View.TimeLine.MinTime)) * renderArea.width;
            else
                x = renderArea.x;
            if (relative)
                x -= renderArea.x;
            return x;
        }
        
        public float GetY(float value, bool relative)
        {
            Rect ra = RenderArea;
            return GetY(value, relative, ref ra);
        }
        protected float GetY(float value, bool relative, ref Rect renderArea)
        {
            float y = (float)(((value - View.MinValue) / (View.MaxValue - View.MinValue)) * renderArea.height);
            y = renderArea.yMax - y;

            if (relative)
                y -= renderArea.yMin;
            return y;
        }
    }
}