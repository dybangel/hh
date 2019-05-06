#!/bin/sh
cat file.txt|while read url 
do
wget $url
done
