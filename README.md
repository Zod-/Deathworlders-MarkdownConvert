## Intro
This tool is made for the online book series [deathworlders](https://github.com/deathworlders/online). It converts markdown to epub and pdf to make distributable versions of the series.

## Requirements
[.NET Core SDK](https://www.microsoft.com/net/download) v2.1.300 or higher.

These packages on linux:
```sh
sudo apt-get install zlib1g fontconfig libfreetype6 libx11-6 libxext6 libxrender1
```

## Installation
```sh
dotnet tool install -g dotnet-deathworlders-md-convert
```

## Usage

```sh
dotnet deathworlders-md-convert chapter.md
dotnet-deathworlders-md-convert chapter.md
```

This will create a `chapter.pdf` and `chapter.epub` on the same path as the markdown file. Out paths is currently not supported.

## Meta information assumptions
The tool tries to collect meta information from several locations. It will try to read `author`, `title` and `date` from a yaml header inside of the `chapter.md`. Furthermore, it will check the folder of the markdown file for a `_index.md` and will extract the same meta information from its yaml header.

When merging those two meta infos it will use different strategies for each field. For `author` it will use the chapter over the index. For `date` it will use the lowest date of them all. And lastly, for `title` it will assume that the index contains the book title while chapter is the chapter title and adds the two together.

Chapters inside each markdown file will be detected by the string `Date Point` and be split inside the resulting epub and pdf accordingly. Those chapters currently will have an incrementing number starting from `Chapter 1`. This can be potentially replaced with a better method such as a hidden nav point element in the markdown with a title for the nav point.

For the last part, it will try to find a cover image file next to the markdown file e.g. `chapter.png`.