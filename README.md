# EasyLingo

> Check the pdf in documents for latest documentation including some screenshots.

## Why

This language expansion module was designed to simplify content editing
when using a large amount of languages in your Sitecore environments.

When you have multiple sites with different language sets (e.g. a
Belgian site in Dutch/French, a Swiss site in fr/de/it and a Danish site
in dk/en) or simply a large volume of available languages, managing
these languages in Sitecore can become cumbersome.

Sitecore has no out of the box functionality that restricts or manages
the languages used per node/site and only offers the languages list to
navigate through the available languages. The language list presented by
Sitecore comprises of both those with one or more versions as well as
those that have no version available or are irrelevant for that
context/site.

## Sitecore

The module (version) is tested with Sitecore 9.0.1. 

## What

This module consists of 2 parts, which can be activated separately. 

### Extra languages section in the content editor
![](https://raw.githubusercontent.com/Gatogordo/EasyLingo/master/images/easylingo-ce-1.jpg)

![](https://raw.githubusercontent.com/Gatogordo/EasyLingo/master/images/easylingo-ce-2.jpg)

### Extra language bar in the Experience editor
The bar can be toggled on or off:

![](https://raw.githubusercontent.com/Gatogordo/EasyLingo/master/images/easylingo-ee-1.png)

Once the bar is toggled on:

![](https://raw.githubusercontent.com/Gatogordo/EasyLingo/master/images/easylingo-ee-1.png)

###Section meaning

The component consists of 4 language lists that are visualized slightly different across the two presentations:
####Versioned languages

The first list shows all the languages that have an actual version available. This is a subset of the languages that are allowed within Sitecore as configured in the System node and/or the “allowedLanguages” setting on the site definition.

####Fallback language versions
The second list contains all the languages for which a fallback version exists (Working through Sitecore’s item fallback available as of version 8.1). The versions with fallback on are rendered in italic. 

####Languages without a version
This third list holds the languages that are available on this node/site, but for which no version exists. This line will not be rendered if there are no such languages. 

####Other available languages
This fourth and final list holds those language versions that are not covered in the ‘allowed languages’ or System node. This could be rogue langue versions that are a remainder of a previous setup or languages created by admin users who are free to create any type of language version.
In these lists, the current language on the Sitecore context is visualized in bold.


Languages in all the lists are clickable
so the editor can easily switch languages without having to go through
the whole list.

In order to get a nice view like in the screenshots it is necessary to
manually set the icons of the languages in the system section of
Sitecore (as Sitecore does not use these icons anymore in the list, they
are not set by default. Flag-icons are still available though).

#### How are “available” languages detected?

We loop through the configured sites and check whether the current item
is part of it (based on the path in the content tree and the “rootPath”
of the site).

For each site we check if a custom property “allowedLanguages” is
available in the SiteSettings specific site node. The distinct union of
all these languages will be the list of allowed languages for the item.
If the list is empty, all available languages in the Sitecore instance
are allowed.

#### How to use the “allowedLanguages” setting?

The parameter will be part of a &lt;site /&gt; definition (just like
hostname, database, rootPath and so on). It is not obligatory – the
module works perfectly without it. It can be used for multi-site
solution with different languages. For our example mentioned above we
could have something like:

&lt; site name=”Belgian” allowedLanguages="nl,fr-BE" … /&gt;

&lt; site name=”Swiss” allowedLanguages="fr,de,it" … /&gt;

&lt; site name=”Danish” allowedLanguages="dk,en" … /&gt;

The language should match the used language exactly (so “fr-BE” ≠ “fr”)
and can be completely different or overlap. For the second part of the
module it is important though to put the “default” language first.

### Second part: request resolver

The second part of the module consists of a request resolver that is put
in the httpRequestBegin pipeline after the LanguageResolver.

The current requested URL is checked to disable the functionality for
Sitecore URL’s or when not in normal page mode. The current context site
is checked for the ‘allowedLanguages’ parameter. If found, and the
current language is not part of the ‘allowedLanguages’ set we will
redirect the user to homepage in the default language (default is the
first language in the set).

## How to enable/disable

There is one configuration file: EasyLingo.config located in
“/include/zEasyLingo”. You will find the request resolver and content
editor section in here. By default both are enabled. To disable, just
comment out the configuration you don’t want.

## Installation

#### Install

EasyLingo can be installed through installation of the EasyLingo package
in Sitecore. The package contains all configurations (enabled by
default) and dll files.

If you prefer, the module is also made available through a ZIP file that
includes all files and should be unzipped in the root folder of your
website.

No files should be overwritten during the installation process.

#### Extra steps

-   Set the flags for your languages in the Sitecore system folder

-   Include the “allowedLanguages” elements to your sites definitions if
    needed

### Future features

There are no specific future features on our tasklist anymore.
If you have specific functional requirements that you would like to see realized through this module, drop us a line or contact us on Twitter, Community, Slack and we will be glad to listen in on your thoughts and observations!

