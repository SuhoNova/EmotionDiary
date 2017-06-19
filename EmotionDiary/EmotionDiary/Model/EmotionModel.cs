using System;
using System.Collections.Generic;
using System.Text;

namespace EmotionDiary.Model
{

        public class FaceRectangle
        {
            public int Top { get; set; }
            public int Left { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }


        public class RootObject
        {
            public FaceRectangle FaceRectangle { get; set; }
            public IDictionary<string, double> scores { get; set; }
        }

}
