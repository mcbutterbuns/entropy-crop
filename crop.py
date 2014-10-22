#!/usr/bin/env python3

import os, sys, argparse
from PIL import Image
from collections import defaultdict
from math import log

size = 150,150


def contentAwareCrop(img, size):
  img = resize(img, size)
  portrait = False
  max_entropy = 0

  #if image is square after thumbnail, we cant crop
  if (img.size[0] == img.size[1]):
    return img

  #if the image is portrait, we'll rotate the image and set a flag reminding
  #us to rotate back afterwards. This makes it easier as we have only one
  #direction to move through the image
  if (img.size[0] < img.size[1]):
    portrait = True
    img.rotate(90)


  #This is the main loop here. Init box with the 0,0 coordinates. We'll calculate
  #the entropy of the cropped image at this point. Then we'll move that box to the right
  #and test the entropy. Continue doing this until we reach the end of the image and
  #use the crop that had the highest entropy
  box = [0, 0, size[0], size[0]]
  best_box = box;

  while box[2] < img.size[0]:

    #crop and calculate the entropy of the portion of the image
    crop = img.crop(tuple(box))
    e = entropy(crop)

    #if this box has the highest entropy, record it
    if (max_entropy < e):
      max_entropy = e
      best_box = list(box)

    #slide the box to the right
    box[0] += 5
    box[2] += 5



  #all done. now crop using the best crop we found
  finalCrop = img.crop(best_box)


  #flip back if the image was in portrat mode
  if portrait:
    finalCrop.rotate(-90)


  #give it back!
  return finalCrop



#resizes the image so that the shortest edge of the image matches the longest edge
#of the desired thumbnail size
def resize(img, size):
  scale = 1

  if (img.size[1] < img.size[0]):
    scale = size[0] / img.size[1]
  else:
    scale = size[1] / img.size[0]

  s = (int(scale * img.size[0]), int(scale * img.size[1]))
  return img.resize(s, Image.ANTIALIAS)


#Calculates the grayscale entropy of an image
def entropy(img):
  entropy = 0
  h = histogram(img)
  area = img.size[0] * img.size[1]

  for x in h.keys():
    p = h[x] / area
    entropy = entropy + abs(p * log(p, 2))

  return entropy




#constructs the histogram of an image
def histogram(img):
  data = list(img.getdata())
  histogram = defaultdict(int)

  for x in range(0, len(data)):
    color = data[x][0] + data[x][1] + data[x][2]
    greyscale = (int)((data[x][0] * .3) + (data[x][1] * .59) + (data[x][2] * .11));

    histogram[color] = histogram[color] + 1

  return histogram



parser = argparse.ArgumentParser(description='Creates a thumbnail from an image.')
parser.add_argument('infile', help='The image to create the thumbnail for')
parser.add_argument('outfile', help='Where to store the thumbnail', type=str)
parser.add_argument('size', nargs=2, help='The thumbnail size', type=int)



if __name__ == "__main__":
  args = parser.parse_args()

  try:
    img = Image.open(args.infile)
    contentAwareCrop(img, args.size).save(args.outfile)
  except IOError as err:
    print("I/O error: {0}".format(err))
