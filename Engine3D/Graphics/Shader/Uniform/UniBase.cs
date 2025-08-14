
using OpenTK.Graphics.OpenGL4;

namespace Engine3D.Graphics.Shader.Uniform
{
    public abstract class UniBase
    {
        protected readonly BaseShader Program;
        protected readonly int Location;

        public UniBase(BaseShader program, string name)
        {
            Program = program;
            Location = program.UniformFind(name);
        }

        public abstract void Update();
    }
}
