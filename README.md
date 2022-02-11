### MPToolKit

## Overview

MPToolKit is a library for mass spectrometry-based proteomics and the home of AScore Pro, an application for scoring and localizing protein post-translational modifications.

## License

This project is covered under the **MIT License**

Third party packages imported in this project may be covered by their own licenses.

## Requirements

MPToolKit is built using Microsoft .NET and targets v3.1 of the .Net Core SDK.  The sources can be compiled and run on Windows, Linux, and macOS with the installation of the corresponding version of the .NET Core SDK and .NET Core runtimes.

The package has been tested on the following operating systems:
- Windows 10
- macOS Monterey 12.1
- Ubuntu 20.04

### AScorePro

## Building

project files can be built with the .NET CLI

Example: (Linux)
```
cd src/AScorePro
dotnet build

# Build Release exe
# Use -r for the runtime that applies to you
dotnet publish -c Release -r win10-x64
```

AScorePro should compile within a few seconds.

## Options

AScorePro requires a parameter file in json format.  The path is passed in using the -j option on the command line.

Example:

`./AScore -j parameters.json`

You may also pass in the scans file with -s and the peptides file with -p.  These override the selection in the parameter file.

`./AScore -j parameters.json -s scans.mzML -p peptides.tsv`

Spectrum file formats are accepted with the following extensions:

- .RAW
- .mzXML
- .mzML

# Parameter file

The contents of the parameter file must be valid json format

Example parameter file:

```
{
    "scans": "scans.mzdb",
    "ion_series": [
        "b",
        "y",
        "nB",
        "nY"
    ],
    "diff_mods": [
        {
            "residues": "M",
            "symbol": "*",
            "mass": 15.994915,
            "n-term": false,
            "c-term": false
        },
        {
            "residues": "STY",
            "symbol": "#",
            "mass": 79.96633,
            "n-term": false,
            "c-term": false
        }
    ],
    "static_mods": [
        {
            "residue": "C",
            "mass": 57.021464,
            "n-term": false
        }
    ],
    "neutral_loss": {
        "mass": -97.9769,
        "residues": "ST"
    },
    "max_peak_depth": 50,
    "tolerance": 0.3,
    "units": "Da",
    "window": 70,
    "low_mass_cutoff": true,
    "deisotope_type": "",
    "filter_low_intensity": 0,
    "no_cterm": true,
    "use_mob_score": true,
    "use_delta_ascore": true,
    "symbol": "#",
    "residues": "STY",
    "out": "out.tsv",
    "max_peptides": 1000,
    "peptides_file": "peptides.tsv"
}
```

Summary

- **scans**: Path to the spectrum file
- **ion_series**: An array containing ion series selection. Any of "a", "b", "c", "x", "y", "z", "nA", "nB", "nY"
- **diff_mods**: Array of diff mod definitions
- **static_mods**: Static mod definitions
- **neutral_loss**: Residues and mass change to consider for fragments creating neutral loss peaks
- **max_peak_depth**: Spectra are filtered for top peaks up to this value per window for scoring
- **tolerance**: Peak match tolerance. Used with units
- **units**: tolerance units  "ppm" or "Da"
- **window**: m/z window size for selecting top peaks
- **low_mass_cutoff**: If true, peaks are not considered if below 0.28 times the max m/z per scan
- **deisotope_type**: Optional. deisotoping strategy. "" or "Top1Per1" or "Match" 
- **filter_low_intensity**: Remove this fraction of lowest intensity peaks. Enter a value between 0 and 1
- **no_cterm**: Set to true to avoid considering mods on peptide c-termini
- **use_mob_score**: Enables use of MOB Score vs original AScore algorithm
- **use_delta_ascore**: Calculate the difference in scores between the top two scoring site positions
- **symbol**: Mod symbol to score. This should match the mod in the diff_mods option
- **residues**: Residues that may contain the target mod
- **out**: Path to output file
- **max_peptides**: Maximum mod permutations to consider per peptide
- **peptides_file**: Path to peptides file

# Peptides file format

The peptides file should contain a list of peptides to score, on per row, formatted as tab-delimited values.  Runtime depends on the number of input peptides, but AScorePro can process thousands of entries per minute on commodity hardware. The ptpdies file should contain the following columns:

- The peptide ID. Can be any value and is copied to the output.
- The scan number.  This should be the scan number in the input file where the peptide was identified.
- the annotated peptide sequence with flanking residues.  Mod symbols should be defined in the parameters.

Example:

```
4304    4916    K.KEES#EES#DDDM*GFGLFD.-        1063.3421469600037
656     866     K.M*LAES#DDS#GDEESVSQTDK.T      1109.8771463738433
```

# Output

The output with be a tab-delimited file with the following columns:

- The peptide ID
- Number of sites scored
- Number of mod permutations considered
- The top scoring annotated peptide
- The score of the top peptide
- Site positions for up to six mods
- Site Scores for up to six mods
