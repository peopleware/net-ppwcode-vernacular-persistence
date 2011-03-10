Dependencies
------------

This directory is intended to be used as svn:external in .net projects.
It contains the definition of a dependency on an external product.

These dependencies are needed for compilation and build, and for running
the code. When you use (parts of) this solutions in other products,
you need the artifacts this solution depends on, recursively, also in
the other product.

When possible, a copy of the required files is kept in this directory
in the repository, so that developers can get started easily. However,
for some dependencies, license issues prohibit us to distribute the
dependencies ourselfs. The developer needs to retrieve them from the
original source himself. In this case, this document describes how to
get the required files.


Microsoft Contracts
-------------------

Version: Release 1.4.31130.0 (November 30, 2010)
(see <http://research.microsoft.com/en-us/projects/contracts/releasenotes.aspx>)

To get the required DLL:
* Download the distribution from
  <http://msdn.microsoft.com/en-us/devlabs/dd491992>.
  We use the "Standard Edition".
  The downloaded file is "Contracts.devlab9std.msi".
* Run the installer (this also installs extra components in Visual Studio)
* Copy the file "Microsoft.Contracts.dll" from
  "C:\Program Files\Microsoft\Contracts\PublicAssemblies\v3.5\"
  to this directory.


  
PPWCode Exceptions Vernacular
-----------------------------

Version: I 2.0 or later

You can use
https://ppwcode.googlecode.com/svn/dotnet/Vernacular/Exceptions/I/latest
from this repository, or a specific version you fill find close by.

These files are released under the Apache License v2.



PPWCode Semantics Vernacular
----------------------------

Version: I 1.4 or later

You can use
https://ppwcode.googlecode.com/svn/dotnet/Util/OdssAndEnds/I/latest
from this repository, or a specific version you will find close by.

These files are released under the Apache License v2.



PPWCode Util OddsAndEnds
------------------------

Version: I 1.0 or later

You can use
https://ppwcode.googlecode.com/svn/dotnet/Vernacular/Semantics/I/latest
from this repository, or a specific version you will find close by.

These files are released under the Apache License v2.


TODO: need to create directories in tools for these dependencies


ANTLR 3
-------

Version: 3.1 or later



Common Logging
--------------

Version: 1.2 or later



HibernatingRhinos.Profiler.Appender
-----------------------------------

Version: 1.0 or later



Iesi Collections
----------------

Version: 1.0 / 2.1.2


log4net
-------

Version: 1.2

These files are released under the Apache License v2.
See the README at https://ppwcode.googlecode.com/svn/dotnet/Tools/log4net/net.2.0/1.n/latest
for more information.



NHibernate
----------

Version: 2.1.2 or later

The NHibernate DLL itself is needed at compile time.

At runtime:
- NHibernate.ByteCode.LinFu, 2.1.2 or later
- NHibernate.Caches.SysCache, 2.1.2 or later
- NHibernate.LambdaExtensions, 1.0.10 or later



Spring.Core
-----------

Version 1.3.1

These files are released under the Apache License v2.
See the README at https://ppwcode.googlecode.com/svn/dotnet/Tools/Spring/Core.2.0/1.n/latest
for more information.



LinFu.DynamicProxy
------------------

Version: 1.0.3 or later

Only needed at runtime.



FluentNHibernate
----------------

Version: 1.0 or later
