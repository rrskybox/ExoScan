ExoScan is Windows desktop software that automates the imaging and the photometric capture of 
exoplanet transits through application of the TheSky™ Professional astroimaging platform. 

Exoplanet transits can be revealed by a dip of apparent magnitude in a host star. The dip is normally 
not more than around 20 or 30 milli-magnitudes. The duration of the transit is on the scale of at most 
a few hours. The rise and fall times are in terms of a few minutes. The magnitude of known 
exoplanet host stars seems to be in the low teens and biased towards reddish spectra. A given size 
exoplanet will probably produce a greater percentage decrease in the relative magnitude in a smaller 
star (dimmer, redder and closer) than a larger star (brighter, bluer and farther). 

Collection of a series of images over several hours from which reliable milli-magnitude stellar 
brightness data can be plotted faces a number of obstacles: seeing variation, air mass extinction, 
temperature variability, and optics imperfections among others. One can largely compensate for 
these sources of image-to-image variability by image noise reduction and field differential photometry. 
That is, using the cataloged spectra of background light sources (stars) for calibrating the apparent 
brightness of the target light source (exoplanet host star), after overall image noise reduction (bias, 
dark and flat). 

Cataloged stellar magnitudes are defined with respect to specific filter color bands. The Gaia catalog 
uses G, Gr and Gb bands. The JPASS catalog uses Johnson-Cousins bands (V, Rj, Bj and Ij). 
Imaged stellar magnitudes will captured by different filter color bands, some quite different, depending 
upon the equipment filters. Differential photometry solves the problem of correlating cataloged stellar 
spectra to imaged spectra by translating color band pairs to a common “standard” color band. The 
application of multiple field stars increases accuracy through statistical analysis. In operation, 
ExoScan begins with the acquisition of one or more target images, taken with at least two filters. 
Light sources in these images are photometrically characterized and astrometrically registered to 
Gaia catalog stars. From the catalog and photometric data, differential color and magnitude 
transformations are computed and used to convert target star image intensities to a chosen standard 
color magnitude.

The software package consists of a session manager for sequencing image capture of target stars, 
and an analysis engine for extracting and translating image star fields into photometric data. The 
session manager controls the acquisition of images for a selected exoplanet target each night. The 
analysis program processes each image to determine the magnitude of the target based on cataloged 
magnitudes of surrounding star field, then transforms instrument magnitudes into standard color
bands. 

A full description of this software can be found in the "publish" directory in ExoScanDescription.pdf.
