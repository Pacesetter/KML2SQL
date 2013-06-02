#KML2SQL v 1.1

###Overview
KML2SQL takes KML files and uploades them to MSSQL database using geography or geometry objects.

[Click Here to Install!](http://goo.gl/TbYhK)

###What's New:
6/2/2013 - A big update that has significantly reworked the backend that allows the following:
* **Improved Security!** I think it's safe against SQL injection, but you should still be careful.
* **Placemark Data!** SimpleData and Data types are now uploaded as additional columns. Note that since Data entries are untyped and SimpleData schemas are unreliable, all data is uploaded into Varchar(max) columns. Oh, and nulls are allowed in all of them. So you end up with a really inefficient database, but converting them all to ints, floats, or whatever is just a few SQL commands away.
* **Improved Expandability!** It should be easier to add support for mySQL and the like in the future.

###To-Do List:

* Provide support for other database types. MSSQL is the first because it has great geospatial support, but getting it working with mySQL is a high priority. If there's interest, I may add PostgreSQL and mongoDB support as well.
* Provide support for polygons with lines that intersect (in real life they shouldn't, but KML lets you draw polygons like that. Code would need to be able to detect that and fix, which would be fairly hard).
* Provide support for KMZ files (but really, those are just zipped KML files, so if you have one of those, just use WinZip for now).

If you have any questions, feel free to post issues here on GitHub, email me at zach(at)zachshuford(dotcom), hit me up on [Google+](https://plus.google.com/100663438782533486183), or tweet me @pharylon. If something isn't working, let me know! If you really need some feature bad, let me know! The amount of work I put into this project and number of features I add is directly proportional to how many people ask for them. 

Lastly, a big "Thank You!" to [SharpKML](http://sharpkml.codeplex.com/) without which this project would be a lot more work and [VectorLady](http://vectorlady.com/) who authored the icon, which is released under the Creative Commons Lisence 3.0 Attributoin.

Finally, KML2SQL itself is licensed under the [BSD 2 Clause License](https://github.com/Pharylon/KML2SQL/blob/master/License.txt). 