using PU_Test.Common.Patch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Launcher.Common.Patch
{
    internal class UA_Util
    {
        #region Byte 操作

        public static readonly long Pas = 0x4889501048BA;
        public static readonly long Pbs = 0x908000000048BA;
        public static readonly byte[] Pbb = { 0x48, 0x89 };
        public static byte[] GetBytesPa(int count)//0-13
        {
            byte[] pa = BitConverter.GetBytes(Pas + (0x80000 * count));
            Array.Resize(ref pa, 6);
            pa = pa.Reverse().ToArray();
            return pa;
        }
        public static byte[] GetBytesPb(int count)//0-28
        {
            byte[] pb = BitConverter.GetBytes(Pbs + (0x80000000000 * count));
            Array.Resize(ref pb, 7);
            byte[] pb2 = Pbb.Concat(pb.Reverse()).ToArray();
            if (count >= 16)
            {
                pb2[2] = 0x90;
                pb2[4] = 0x01;
            }
            return pb2;
        }

        #endregion

        public static byte[] ToUABytes(string key)
        {
            int count = key.Length + 2;
            List<byte> uabytes = Encoding.UTF8.GetBytes(key).ToList();
            for (int i = 1; i < 44; i++)
            {
                if (i <= 29)
                {
                    byte[] k = GetBytesPb(29 - i);
                    for (int j = 0; j < k.Length; j++)
                    {
                        uabytes.Insert(count - 8 * i, k[k.Length - 1 - j]);
                    }
                }
                else
                {
                    byte[] k = GetBytesPa(14 - (i - 29));
                    for (int j = 0; j < k.Length; j++)
                    {
                        uabytes.Insert(count - 8 * i, k[k.Length - 1 - j]);
                    }
                }
            }
            return uabytes.ToArray();
        }


        public const string dispatchKey_FILE_UA = @"key/dispatchKey_UA.txt";
        public const string dispatchKey_FILE_UA_Old = @"key/dispatchKey_UA_Old.txt";
        public static void Patch_Bytes(ref byte[] filebytes)
        {
            string dispatchKeyUA = File.ReadAllText(dispatchKey_FILE_UA);
            string dispatchKeyUAOld = File.ReadAllText(dispatchKey_FILE_UA_Old);

            filebytes =PatchHelper.ReplaceBytes(filebytes, ToUABytes(dispatchKeyUAOld), ToUABytes(dispatchKeyUA));
            

        }
        public static void Patch_File(string inpath, string outpath)
        {
            byte[] data = File.ReadAllBytes(inpath);
            Patch_Bytes(ref data);
            FileStream stream = File.Create(outpath);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }
    }
}
