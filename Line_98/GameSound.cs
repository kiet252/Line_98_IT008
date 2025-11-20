using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Line_98
{
    internal class GameSound
    {
        internal static void PlayDestroySound()
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.ding); // Âm thanh ding.wav
            player.Play();
        }

        internal static void PlayCantMoveSound()
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.cantmove); // Âm thanh cantmove.wav
            player.Play();
        }

        internal static void PlayMoveSound()
        {
            SoundPlayer player = new SoundPlayer(Properties.Resources.move); // Âm thanh move.wav
            player.Play();
        }
    }
}
