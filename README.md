entropy-crop
============

Simple python script to crop images to a certain size with the added twist that
it will scan the image and select the crop section based upon the entropy in the
image.


    ./crop.py samples/island.jpg samples/island_thumb.jpg 150 150

<img src="https://raw.githubusercontent.com/mcbutterbuns/entropy-crop/master/samples/island.jpg" alt="Island" style="width:300px">

Becomes

![Island Thumbnail](https://raw.githubusercontent.com/mcbutterbuns/entropy-crop/master/samples/island_thumb.jpg)
