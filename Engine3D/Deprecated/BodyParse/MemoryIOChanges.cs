using System;
using System.Collections.Generic;

using Engine3D.Abstract3D;

namespace Engine3D.BodyParse
{
    class MemoryIOChanges
    {
        private CBodyParserData BodyParser;

        public MemoryIOChanges(CBodyParserData body_parser)
        {
            BodyParser = body_parser;
        }

        public void Change(Point3D p)
        {
            BodyParser.Template.Edit_Insert_Corner(p);
            BodyParser.IndexInfo.Length_Corner = (uint)BodyParser.Template.CornerCount();
        }
        public void Change(IndexTriangle f)
        {
            BodyParser.Template.Edit_Insert_Face(f);
            BodyParser.IndexInfo.Length_Face = (uint)BodyParser.Template.FaceCount();
        }
        public void Change(uint idx, uint col)
        {
            BodyParser.Template.Edit_Change_Color(idx, col);
        }
    }
}
