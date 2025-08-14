using System;
using System.Collections.Generic;
using System.IO;

using Engine3D.Abstract.Simple;
using Engine3D.Abstract.Complex;
using Engine3D.OutPut.Shader;

using Engine3D.StringParse;

using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Abstract
{
    public enum ECornerType
    {
        None = 0,
        YXC = 1,
    };
    public interface ICorner
    {

    }
    public struct SCorner_YXC : ICorner
    {
        public float Y;
        public float X;
        public float C;

        public SCorner_YXC(float y, float x, float c)
        {
            Y = y;
            X = x;
            C = c;
        }
    }

    public enum EFaceType
    {
        None = 0,
        ABC_Col = 1,
    };
    public interface IFace
    {

    }
    public struct SFace_ABC_Col : IFace
    {
        public uint A;
        public uint B;
        public uint C;

        public uint Color;

        public SFace_ABC_Col(uint a, uint b, uint c, uint color)
        {
            A = a; B = b; C = c;
            Color = color;
        }
    }



    public class CBody
    {
        private readonly ECornerType CornerType;
        private readonly EFaceType FaceType;

        private ICorner[] CornerArr;
        private IFace[] FaceArr;

        public int CornerCount { get { return CornerArr.Length; } }
        public int FaceCount { get { return FaceArr.Length; } }

        private CBody(ECornerType cornerType, List<ICorner> cornerList, EFaceType faceType, List<IFace> faceList)
        {
            CornerType = cornerType;
            CornerArr = cornerList.ToArray();

            FaceType = faceType;
            FaceArr = faceList.ToArray();
        }

        public struct SBodyTemplate
        {
            public readonly ECornerType CornerType;
            public readonly EFaceType FaceType;

            public List<ICorner> CornerList;
            public List<IFace> FaceList;

            public SBodyTemplate(ECornerType cornerType, EFaceType faceType)
            {
                CornerType = cornerType;
                FaceType = faceType;

                CornerList = new List<ICorner>();
                FaceList = new List<IFace>();
            }

            public void Insert(ICorner corner)
            {
                CornerList.Add(corner);
            }
            public void Insert(IFace face)
            {
                FaceList.Add(face);
            }

            public CBody ToBody()
            {
                return new CBody(CornerType, CornerList, FaceType, FaceList);
            }
        }

        public CBodyBuffer ToBuffer()
        {
            CBodyBuffer buffer = new CBodyBuffer("test", CornerType, FaceType);

            buffer.Corner_Func(CornerArr);
            buffer.Face_Func(FaceArr);

            return buffer;
        }
        public BoundBox CalcBox()
        {
            Punkt Min = new Punkt(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Punkt Max = new Punkt(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

            for (int i = 0; i < CornerArr.Length; i++)
            {
                SCorner_YXC corn = (SCorner_YXC)CornerArr[i];

                if (corn.Y < Min.Y) { Min.Y = corn.Y; }
                if (corn.X < Min.X) { Min.X = corn.X; }
                if (corn.C < Min.C) { Min.C = corn.C; }

                if (corn.Y > Max.Y) { Max.Y = corn.Y; }
                if (corn.X > Max.X) { Max.X = corn.X; }
                if (corn.C > Max.C) { Max.C = corn.C; }
            }

            return BoundBox.MinMax(Min, Max);
        }
        public Punkt CalcAverage()
        {
            Punkt Avg = new Punkt();

            for (int i = 0; i < CornerArr.Length; i++)
            {
                SCorner_YXC corn = (SCorner_YXC)CornerArr[i];

                Avg.Y += corn.Y / i;
                Avg.X += corn.X / i;
                Avg.C += corn.C / i;
            }

            return Avg;
        }

        public bool Intersekt(Ray ray, Transformation trans, out double distance, out int faceIndex)
        {
            distance = double.PositiveInfinity;
            faceIndex = -1;

            for (int i = 0; i < FaceArr.Length; i++)
            {
                SFace_ABC_Col face = (SFace_ABC_Col)FaceArr[i];
                SCorner_YXC a = (SCorner_YXC)CornerArr[face.A];
                SCorner_YXC b = (SCorner_YXC)CornerArr[face.B];
                SCorner_YXC c = (SCorner_YXC)CornerArr[face.C];

                double d = ray.Dreieck_Schnitt_Interval(
                    trans.TFore(new Punkt(a.Y, a.X, a.C)),
                    trans.TFore(new Punkt(b.Y, b.X, b.C)),
                    trans.TFore(new Punkt(c.Y, c.X, c.C))
                    );
    
                if (d > 0.0 && d < distance)
                {
                    distance = d;
                    faceIndex = i;
                }
            }

            return (faceIndex != -1);
        }
    }
    public class CBodyBuffer : CBuffer
    {
        private readonly ECornerType CornerType;
        private readonly EFaceType FaceType;

        private int Corner_YXC;
        private int Face_ABC;
        private int Face_Color;

        private int Index_Count;

        public CBodyBuffer(string name, ECornerType cornerType, EFaceType faceType) : base(name)
        {
            CornerType = cornerType;
            FaceType = faceType;

            if (CornerType == ECornerType.YXC)
            {
                Corner_YXC = GL.GenBuffer();
            }

            if (FaceType == EFaceType.ABC_Col)
            {
                Face_ABC = GL.GenBuffer();
                Face_Color = GL.GenBuffer();
            }

            Index_Count = 0;
        }
        ~CBodyBuffer()
        {
            Use();

            GL.DeleteBuffer(Corner_YXC);
            GL.DeleteBuffer(Face_ABC);
            GL.DeleteBuffer(Face_Color);
        }

        public void Corner_Func(ICorner[] cornerArr)
        {
            Use();

            if (CornerType == ECornerType.YXC)
            {
                float[] data_YXC = new float[cornerArr.Length * 3];
                int size_YXC = 0;
                for (int i = 0; i < cornerArr.Length; i++)
                {
                    SCorner_YXC corner = (SCorner_YXC)cornerArr[i];
                    data_YXC[size_YXC] = corner.Y; size_YXC++;
                    data_YXC[size_YXC] = corner.X; size_YXC++;
                    data_YXC[size_YXC] = corner.C; size_YXC++;
                }
                size_YXC = size_YXC * sizeof(float);

                GL.BindBuffer(BufferTarget.ArrayBuffer, Corner_YXC);
                GL.BufferData(BufferTarget.ArrayBuffer, size_YXC, data_YXC, BufferUsageHint.StaticDraw);
            }

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }
        public void Face_Func(IFace[] faceArr)
        {
            Use();

            if (FaceType == EFaceType.ABC_Col)
            {
                uint[] data_ABC = new uint[faceArr.Length * 3];
                uint[] data_Col = new uint[faceArr.Length];
                int size_ABC = 0;
                int size_Col = 0;
                for (int i = 0; i < faceArr.Length; i++)
                {
                    SFace_ABC_Col face = (SFace_ABC_Col)faceArr[i];
                    data_ABC[size_ABC] = face.A; size_ABC++;
                    data_ABC[size_ABC] = face.B; size_ABC++;
                    data_ABC[size_ABC] = face.C; size_ABC++;
                    data_Col[size_Col] = face.Color; size_Col++;
                }
                size_ABC = size_ABC * sizeof(uint);
                size_Col = size_Col * sizeof(uint);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, Face_ABC);
                GL.BufferData(BufferTarget.ElementArrayBuffer, size_ABC, data_ABC, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Face_Color);
                GL.BufferData(BufferTarget.ShaderStorageBuffer, size_Col, data_Col, BufferUsageHint.StaticDraw);

                Index_Count = faceArr.Length * 3;
            }
        }

        public override void Draw()
        {
            Use();

            if (FaceType == EFaceType.ABC_Col)
            {
                GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, Face_Color);
                GL.DrawElements(BeginMode.Triangles, Index_Count, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
    public static class TBodyGenerate
    {
        public static CBody CubeBody(float scale = 1.0f)
        {
            CBody.SBodyTemplate template = new CBody.SBodyTemplate(ECornerType.YXC, EFaceType.ABC_Col);

            template.Insert(new SCorner_YXC(-scale, -scale, -scale));
            template.Insert(new SCorner_YXC(+scale, -scale, -scale));
            template.Insert(new SCorner_YXC(-scale, +scale, -scale));
            template.Insert(new SCorner_YXC(+scale, +scale, -scale));
            template.Insert(new SCorner_YXC(-scale, -scale, +scale));
            template.Insert(new SCorner_YXC(+scale, -scale, +scale));
            template.Insert(new SCorner_YXC(-scale, +scale, +scale));
            template.Insert(new SCorner_YXC(+scale, +scale, +scale));

            template.Insert(new SFace_ABC_Col(0, 2, 1, 0x0000FF));
            template.Insert(new SFace_ABC_Col(2, 3, 1, 0x0000FF));
            template.Insert(new SFace_ABC_Col(7, 6, 5, 0x0000FF));
            template.Insert(new SFace_ABC_Col(6, 4, 5, 0x0000FF));

            template.Insert(new SFace_ABC_Col(0, 4, 2, 0xFF0000));
            template.Insert(new SFace_ABC_Col(4, 6, 2, 0xFF0000));
            template.Insert(new SFace_ABC_Col(7, 5, 3, 0xFF0000));
            template.Insert(new SFace_ABC_Col(5, 1, 3, 0xFF0000));

            template.Insert(new SFace_ABC_Col(0, 1, 4, 0x00FF00));
            template.Insert(new SFace_ABC_Col(1, 5, 4, 0x00FF00));
            template.Insert(new SFace_ABC_Col(7, 3, 6, 0x00FF00));
            template.Insert(new SFace_ABC_Col(3, 2, 6, 0x00FF00));

            return template.ToBody();
        }
        public static CBody ErrorBody()
        {
            CBody.SBodyTemplate template = new CBody.SBodyTemplate(ECornerType.YXC, EFaceType.ABC_Col);

            template.Insert(new SCorner_YXC( 0, +1,  0));
            template.Insert(new SCorner_YXC(+1,  0,  0));
            template.Insert(new SCorner_YXC( 0,  0, +1));
            
            template.Insert(new SCorner_YXC( 0, -1,  0));
            template.Insert(new SCorner_YXC(-1,  0,  0));
            template.Insert(new SCorner_YXC( 0,  0, -1));

            template.Insert(new SFace_ABC_Col(0, 1, 2, 0x000000));
            template.Insert(new SFace_ABC_Col(0, 5, 1, 0xFF0000));
            template.Insert(new SFace_ABC_Col(0, 2, 4, 0x0000FF));
            template.Insert(new SFace_ABC_Col(3, 2, 1, 0x00FF00));

            template.Insert(new SFace_ABC_Col(0, 4, 5, 0xFF00FF));
            template.Insert(new SFace_ABC_Col(3, 1, 5, 0xFFFF00));
            template.Insert(new SFace_ABC_Col(3, 4, 2, 0x00FFFF));
            template.Insert(new SFace_ABC_Col(3, 5, 4, 0xFFFFFF));

            uint idx = 6;

            template.Insert(new SCorner_YXC(0, +0.5f, 0));
            template.Insert(new SCorner_YXC(+0.5f, 0, 0));
            template.Insert(new SCorner_YXC(0, 0, +0.5f));

            template.Insert(new SCorner_YXC(0, -0.5f, 0));
            template.Insert(new SCorner_YXC(-0.5f, 0, 0));
            template.Insert(new SCorner_YXC(0, 0, -0.5f));

            template.Insert(new SFace_ABC_Col(idx + 0, idx + 2, idx + 1, 0x000000));
            template.Insert(new SFace_ABC_Col(idx + 0, idx + 1, idx + 5, 0xFF0000));
            template.Insert(new SFace_ABC_Col(idx + 0, idx + 4, idx + 2, 0x0000FF));
            template.Insert(new SFace_ABC_Col(idx + 3, idx + 1, idx + 2, 0x00FF00));

            template.Insert(new SFace_ABC_Col(idx + 0, idx + 5, idx + 4, 0xFF00FF));
            template.Insert(new SFace_ABC_Col(idx + 3, idx + 5, idx + 1, 0xFFFF00));
            template.Insert(new SFace_ABC_Col(idx + 3, idx + 2, idx + 4, 0x00FFFF));
            template.Insert(new SFace_ABC_Col(idx + 3, idx + 4, idx + 5, 0xFFFFFF));

            return template.ToBody();
        }
    }
}
