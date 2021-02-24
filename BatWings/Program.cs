using System.Collections.Generic;
using System.IO;

namespace BatWings
{
    class Program
    {

        static void Main(string[] args)
        {
            /**
             * General concept:
             * Unity (well, the version used by VRChat) only supports up to 4 bones per vertex.
             * Therefore I use this basic algorithm to generate the weights for bat-like wings
             * by limiting the bones per vertex to 2 from the body side and 2 from the finger side.
             * The hard edges in the weight map are then smoothed and all weights normalized.
             * Then the weight maps are exported to PNG images that can be imported to Blender
             * using some plugins.
             * 
             * It works for me but I can't promise that it works well for anyone else. Use at your own risk!
             * 
             * The code below generates the weights for the wing membrane between body and last finger.
             * It shouldn't be too hard to adapt for the other wing membranes
             */


            // By default the images will be saved in the bin directory
            string exportPath = Directory.GetCurrentDirectory();
            string exportBaseName = Path.Combine(exportPath + "bone");

            double edgeSmoothingFac = 10;

            int exportedImageRes = 100;

            /*
             * One bone for the entire body and one for the entire pinky.
             * This will define the general influence from the body towards the finger and the other way
             */
            HashSet<Bone> bonesBodyFinger = new HashSet<Bone>
            {
                /**
                 * Find the bone start and end positions (in pixel) on your UV map, enter them here.
                 * Change 1024 to the pixel size of your UV map.
                 */
                new Bone("body", new Vector(470, 1020) / 1024, new Vector(1024, 1020) / 1024),
                new Bone("finger", new Vector(542, 585) / 1024, new Vector(995, 553) / 1024)
            };
            WeightMap weightMapBodyFinger = new WeightMap(exportedImageRes, exportedImageRes);
            weightMapBodyFinger.CalculateDistances(bonesBodyFinger);
            //weightMapBodyFinger.ExportToImage(exportBaseName);


            
            // This defines the body bones
            HashSet<Bone> bonesBodyDetail = new HashSet<Bone>
            {
                // I did the weights of the arm by hand (I don't know if this actually works well)
                //new Bone("upperArm", new Vector(562, 968) / 1024, new Vector(549, 795) / 1024),

                new Bone("upperleg", new Vector(1024, 1020) / 1024, new Vector(852, 1020) / 1024),
                new Bone("hips", new Vector(825, 1020) / 1024, new Vector(733, 1020) / 1024),

                // I got better results by combining spine and chest: 1 bone less
                //new Bone("spine", new Vector(733, 1020) / 1024, new Vector(665, 1020) / 1024),
                //new Bone("chest", new Vector(665, 1020) / 1024, new Vector(470, 1020) / 1024) 
                new Bone("spine", new Vector(733, 1020) / 1024, new Vector(470, 1020) / 1024)
            };
            WeightMap weightMapBodyDetail = new WeightMap(exportedImageRes, exportedImageRes);
            weightMapBodyDetail.CalculateDistances(bonesBodyDetail);
            weightMapBodyDetail.LimitWeightCount(2);
            weightMapBodyDetail.SmoothEdge("upperleg", new Bone("", new Vector(.78, 0), new Vector(.78, 1)), edgeSmoothingFac);
            weightMapBodyDetail.SmoothEdge("spine", new Bone("", new Vector(.78, 0), new Vector(.78, 1)), edgeSmoothingFac);
            //weightMapBodyDetail.SmoothEdge("hips", new Bone("", new Vector(.69, 0), new Vector(.69, 1)), edgeSmoothingFac);
            //weightMapBodyDetail.SmoothEdge("chest", new Bone("", new Vector(.69, 0), new Vector(.69, 1)), edgeSmoothingFac);
            weightMapBodyDetail.Normalize();
            weightMapBodyDetail.Multiply(weightMapBodyFinger, "body");
            weightMapBodyDetail.ExportToImage(exportBaseName);



            // This defines the pinky bones
            HashSet<Bone> bonesBodyFingerDetail = new HashSet<Bone>
            {
                // I did the weights of the arm by hand (I don't know if this actually works well)
                //new Bone("lowerArm", new Vector(549, 795) / 1024, new Vector(538, 610) / 1024),

                new Bone("pinky1", new Vector(542, 585) / 1024, new Vector(680, 567) / 1024),
                new Bone("pinky2", new Vector(680, 567) / 1024, new Vector(838, 556) / 1024),
                new Bone("pinky3", new Vector(838, 556) / 1024, new Vector(995, 553) / 1024)
            };
            WeightMap weightMapFingerDetail = new WeightMap(exportedImageRes, exportedImageRes);
            weightMapFingerDetail.CalculateDistances(bonesBodyFingerDetail);
            weightMapFingerDetail.LimitWeightCount(2);
            weightMapFingerDetail.SmoothEdge("pinky1", new Bone("", new Vector(.71, 0), new Vector(.77, 1)), edgeSmoothingFac);
            weightMapFingerDetail.SmoothEdge("pinky3", new Bone("", new Vector(.71, 0), new Vector(.77, 1)), edgeSmoothingFac);
            weightMapFingerDetail.Normalize();
            weightMapFingerDetail.Multiply(weightMapBodyFinger, "finger");
            weightMapFingerDetail.ExportToImage(exportBaseName);

        }
    }
}
