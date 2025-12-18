using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Line_98 {
    static class SaveAndLoad {

        /// <summary>
        /// Lưu game dùng BinaryWriter 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="g"></param>
        public static void Save(string filename, MainGamePanel g) {
            using (BinaryWriter w = new BinaryWriter(File.Open(filename, FileMode.Create))) {
                w.Write(g.GameScore);
                w.Write(g.GameTime);

                for (int i = 0; i < 9; ++i) {
                    for (int j = 0; j < 9; j++) {
                        w.Write(g.GameBoardColor[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Load game dùng BinaryReader
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="g"></param>
        public static void Load(string filename, MainGamePanel g) {
            using (BinaryReader r = new BinaryReader(File.Open(filename, FileMode.Open))) { 
                g.GameScore = r.ReadInt32();
                g.GameTime = r.ReadInt32();

                for (int i = 0; i < 9; ++i) {
                    for (int j = 0; j < 9; j++) {
                        g.setGameBoardColor(i, j, r.ReadInt32());
                    }
                }
            }

            g.Invalidate();
            g.RepaintCurrentScore();
        }
    }
}
