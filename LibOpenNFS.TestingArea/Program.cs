using System;
using System.IO;
using LibOpenNFS.Utils;
using LibOpenNFS.VFS;

namespace LibOpenNFS.TestingArea
{
    internal static class Program
    {
        public static unsafe void Main(string[] args)
        {
            // Experimental functions
            {
                if (File.Exists("STREAML5RA_0x0E5EA4D0.BUN"))
                {
                    using (var fs = new BinaryReader(File.OpenRead("STREAML5RA_0x0E5EA4D0.BUN")))
                    {
                        for (var i = 0; i < 34; i++)
                        {
                            var xBytes = fs.ReadBytes(4);
                            var yBytes = fs.ReadBytes(4);
                            var zBytes = fs.ReadBytes(4);

                            Array.Reverse(xBytes);
                            Array.Reverse(zBytes);

                            var x = BitConverter.ToSingle(xBytes, 0);
                            var y = BitConverter.ToSingle(yBytes, 0);
                            var z = BitConverter.ToSingle(zBytes, 0);

                            Console.WriteLine(
                                $"{xBytes[0]:X2} {xBytes[1]:X2} {xBytes[2]:X2} {xBytes[3]:X2} -> {Convert.ToString(BitConverter.ToInt32(xBytes, 0), 2).PadLeft(32, '0')}");
                            Console.WriteLine(
                                $"{yBytes[0]:X2} {yBytes[1]:X2} {yBytes[2]:X2} {yBytes[3]:X2} -> {Convert.ToString(BitConverter.ToInt32(yBytes, 0), 2).PadLeft(32, '0')}");
                            Console.WriteLine(
                                $"{zBytes[0]:X2} {zBytes[1]:X2} {zBytes[2]:X2} {zBytes[3]:X2} -> {Convert.ToString(BitConverter.ToInt32(zBytes, 0), 2).PadLeft(32, '0')}");

//                            Console.WriteLine(x < -0.69783568f ? "less" : "greater or equal");

                            if (x < -0.69783568f || x > 0.70934081f || float.IsNaN(x))
                            {
                                fixed (byte* fp = xBytes)
                                {
                                    Console.WriteLine($"unpacked = {BinaryUtil.GetPackedFloat(fp, 0)}");
                                }
                            }
                            else
                            {
                                var exponent = x == 0.0f ? 0 : (int) Math.Floor((Math.Log10(Math.Abs(x))));

                                if (exponent < 0)
                                {
                                    fixed (byte* fp = xBytes)
                                    {
                                        Console.WriteLine($"unpacked (exponent) = {BinaryUtil.GetPackedFloat(fp, 0)}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"value: {x}");
                                }
                            }

                            Console.WriteLine();
                            fs.BaseStream.Seek(20, SeekOrigin.Current);
                        }
                    }
                }
            }

            // VFS testing
            {
                var vfsManager = VfsManager.Instance;

                var bundle1 = VfsManager.CreateBundle("TestBundle");
                var bundle2 = VfsManager.CreateBundle("TestBundle2");
                var bundle3 = VfsManager.CreateBundle("TestBundle3");

                bundle1.MountResource(VfsManager.CreateTexturePackResource());
                bundle1.MountResource(VfsManager.CreateTexturePackResource());
                bundle3.MountResource(VfsManager.CreateTexturePackResource());

                vfsManager.MountBundle("/", bundle1);
                vfsManager.MountBundle("/Global", bundle1);
                vfsManager.MountBundle("/Global", bundle2);
                vfsManager.MountBundle("/Global/Test", bundle3);
                
                vfsManager.UnmountBundle($"/Global/Test/{bundle3.ID}");
            }

            if (args.Length > 0)
            {
                // GameManager testing
                var gameManager = GameManager.Instance;

                gameManager.LoadGame(args[0]);

                gameManager.GetLoader().LoadMapStream();
            }
        }
    }
}