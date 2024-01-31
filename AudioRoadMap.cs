using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Somnisonus
{
    public class AudioRoadMap
    {
        AudioCollection? nowPlaying;
        public AudioRoadMap()
        {
            nowPlaying = null;
        }

        public void SetCollection(AudioCollection collection)
        {
            nowPlaying = collection;
        }

        public AudioCollection GetNowPlaying()
        {
            return nowPlaying;
        }

        public static readonly AudioRoadMap Instance = new AudioRoadMap();
    }
}
