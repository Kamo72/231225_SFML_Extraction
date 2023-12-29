using Microsoft.VisualBasic;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _231109_SFML_Test
{
    internal class IngameBackgroundDrawer : IDisposable
    {
        static IngameBackgroundDrawer() 
        {
            bgTexDic = new Dictionary<BackgroundType, RectangleShape>();
            bgTexDic[BackgroundType.NONE] = null;
            bgTexDic[BackgroundType.GRASS] = new RectangleShape(new Vector2f(backgroundSep, backgroundSep));
            bgTexDic[BackgroundType.GRASS].Texture = ResourceManager.textures["texGrass"];
            bgTexDic[BackgroundType.CONCRETE] = new RectangleShape(new Vector2f(backgroundSep, backgroundSep));
            bgTexDic[BackgroundType.CONCRETE].Texture = ResourceManager.textures["texConcrete"];

            backgroundLoaded = new Dictionary<Vector2i, BackgroundType>
            {
                { new Vector2i(0, 3), BackgroundType.CONCRETE },
                { new Vector2i(0, -3), BackgroundType.CONCRETE },
                { new Vector2i(3, 0), BackgroundType.CONCRETE },
                { new Vector2i(-3, 0), BackgroundType.CONCRETE }
            };

        }
        public IngameBackgroundDrawer(BackgroundType basicType = BackgroundType.NONE) 
        {
            this.basicType = basicType;
            nodes = new Dictionary<Vector2i, BackgroundNode>();
        }


        #region [배경 노드]
        public enum BackgroundType
        {
            NONE,
            GRASS,
            CONCRETE,
        }

        public const float backgroundSep = 200f;
        public float backgroundDrawRange = 2000f;

        public readonly BackgroundType basicType;

        public static Random rotChooser = new Random();

        //그리기 객체
        struct BackgroundNode 
        {
            public BackgroundNode(Vector2i pos, BackgroundType type)
            {
                integerPos = pos;
                this.type = type;
                rot = rotChooser.Next(4);
                isActivated = true;
            }

            public Vector2i integerPos;
            public BackgroundType type;
            public int rot;
            public bool isActivated;

            public void Disable() { isActivated = false; }

        }

        //현재 활성화된 노드 목록(좌표, 유형, 그리기 객체)
        Dictionary<Vector2i, BackgroundNode> nodes;
        List<BackgroundNode> nodesToDraw = new List<BackgroundNode>();
        //배경 유형 > 배경 텍스쳐
        static Dictionary<BackgroundType, RectangleShape> bgTexDic;
        //활성화에 무관한 모든 노드 목록(좌표, 유형)
        static Dictionary<Vector2i, BackgroundType> backgroundLoaded;

        //활성화된 모든 노드를 Draw();
        public void DrawBackgroundProcess()
        {
            //Console.WriteLine("DrawBackgroundProcess - " + "start" + nodes.Count);

            RenderStates rs = CameraManager.worldRenderState;
            //lock(nodesToDraw)

            List<BackgroundNode> nodesToDrawT;
            lock (nodesToDraw) nodesToDrawT= nodesToDraw;

            //Console.WriteLine("nodesToDraw - Draw Start" + nodesToDraw.Count);
            
                for (int i = 0; i < nodesToDrawT.Count; i++)
                {
                    BackgroundNode node = nodesToDrawT[i];

                    RectangleShape rect = bgTexDic[node.type];
                    if (rect == null) continue;
                    rect.Position = (Vector2f)node.integerPos * backgroundSep;
                    rect.Rotation = node.rot * 90f;
                    rect.Origin = new Vector2f(backgroundSep, backgroundSep) / 2f;

                    //Console.WriteLine("nodesToDraw - Draw" + node.integerPos);
                    DrawManager.texWrBackground.Draw(rect, rs);
                }
        }

        Thread refreshTask;

        //비활성화 대상을 비활성화. 활성화 대상을 활성화
        public void RefreshBackgroundProcess() 
        {
            if (refreshTask != null) { Console.WriteLine("RefreshBackgroundProcess - aborted"); return; }


            refreshTask = new Thread(() =>
            {
                List<BackgroundNode> nodesToDrawT = new List<BackgroundNode>();

                //Console.WriteLine("nodesToDraw - Reset");
                //nodesToDraw.Clear();
                backgroundDrawRange = (float)Math.Sqrt(CameraManager.size.X * CameraManager.size.X + CameraManager.size.Y * CameraManager.size.Y) * CameraManager.zoomValue;

                //추가
                Vector2f posCameraF = CameraManager.position;
                Vector2i posCamera = new Vector2i((int)(posCameraF.X / backgroundSep), (int)(posCameraF.Y / backgroundSep));

                int bgRange = (int)(backgroundDrawRange / backgroundSep);
                for (int xPick = posCamera.X - bgRange; xPick < posCamera.X + bgRange; xPick++)
                    for (int yPick = posCamera.Y - bgRange; yPick < posCamera.Y + bgRange; yPick++)
                    {
                        Vector2i pickPos = new Vector2i(xPick, yPick);

                        float len = ((Vector2f)pickPos * backgroundSep - CameraManager.position).Magnitude();
                        if (len > backgroundDrawRange) continue;

                        //만약에 이미 nodes에 존재한다면, 넘어간다.
                        BackgroundNode output;
                        if (nodes.TryGetValue(pickPos, out output))
                        {
                            //Console.WriteLine("nodesToDraw - Add (ret == true");
                            nodesToDrawT.Add(output);
                            continue;
                        }

                        //없다면 새로 생성해 추가
                        BackgroundType type;
                        type = backgroundLoaded.TryGetValue(pickPos, out type) ? type : BackgroundType.GRASS;

                        BackgroundNode node = new BackgroundNode(pickPos, type);
                        nodes.Add(pickPos, node);

                        //Console.WriteLine("nodesToDraw - Add new");
                        nodesToDrawT.Add(node);
                    }

                lock (nodesToDraw)
                    nodesToDraw = nodesToDrawT;

                //Console.WriteLine("RefreshBackgroundProcess" + nodesToDraw.Count);
                refreshTask = null;

            });
            refreshTask.Start();

        }
        #endregion


        public void Dispose()
        {
            refreshTask?.Abort();
            GC.SuppressFinalize(this);
        }
    }
}
