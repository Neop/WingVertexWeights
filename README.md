# WingVertexWeights
A very basic algorithm for generating vertex weights for flat surfaces with bone limit.

I use it for generating vertex weights for bat-like wings on models for Unity. I uploaded it here in case it's useful for anyone, however I can't promise that it works well. Consider it an experiment and use at your own risk! You will also need to do some cleanup work on the final weights but that should be easier than doing it all by hand.

Some basic coding knowledge is recommended since bone coordinates are defined in the code. You'll probably need Visual Studio or some C# compiler.

## Why

Unity (well, the version currently used by VRChat) only supports up to 4 bones per vertex. If there are more bones some will be discarded, which can cause parts of the object to become stiff. Doing the weights by hand or fixing the automatic weighs is too difficult / I'm too lazy.

## Concept

I use this (very) basic algorithm to generate the weights for bat-like wings by limiting the bones per vertex to 2 from the body side and 2 from the finger side: I generate the weight maps for the body bones and separately from that weight maps for the pinky bones. Then I generate a weight map with one bone representing the entire body and one representing the finger. This is used to multiply a falloff onto the body and finger bones.

Since limiting the bone count per pixel causes hard edges in the weight map I included a basic way to smooth along a line. In more complex cases this could be done using an image manipulation program (make sure that 100% black pixels stay black!)

For each bone a weigh map (PNG image) is exported that can be imported into blender using some plugins (see below).

## How to use

The bones need to be defined in source code. Check the comments in Program.cs on how to do that.
Remove / comment the SmoothEdge() calls at first. Then check for lines to be smoothed. Use an image manipulation program to find the pixel coordinates where the edges meet the edge of the image and enter those into the SmoothEdge() calls.

Importing the weights into Blender is a little tricky, as far as I know there's no direct way to import images into vertex weights (if there is, please tell me! xD).
- Open your model in Blender 2.8+
- Use this plugin to import the weight maps to vertex colors: https://github.com/danielenger/Bake-to-Vertex-Color
- Export your model to fbx and import to Blender 2.79
- Use this plugin to convert vertex colors to vertex weights: https://github.com/chebhou/Weight-and-Color
- Save as .blend file
- Open with Blender 2.8+ or append the object from the .blend file
- Do some cleanup by hand if needed and add weights for the arm bones if needed (I found this was easier than dealing with non-linear hard edges in the weights)
