#KML2SQL v 1.0

###Overview
KML2SQL is a program that takes KML files and uploades them to MSSQL geography or geometry files. I'm now feeling good enough about this that I'm going to call it Version 1.0.

[Click Here to Install!](http://goo.gl/arnVS)

###WARNING:
KML2SQL is vulnerable to SQL injection attacks. Fixing this is on the to-do list, but you should not be using KML files that might be malicious.

To-Do List:

* Add support for <SimpleData> and <Timespan> attributes as extra columns in SQL.
* Provide support for other database types. MSSQL is the first because it has great geospatial support, but getting it working with mySQL is a high priority. If there's interest, I may add PostgreSQL and mongoDB support as well.
* Provide support for polygons with lines that intersect (in real life they shouldn't, but KML lets you draw polygons like that. Code would need to be able to detect that and fix, which would be fairly hard).

If you have any questions, feel free to post issues here on GitHub, email me at zach(at)zachshuford(dotcom), hit me up on [Google+](https://plus.google.com/100663438782533486183), or tweet me @pharylon. If something isn't working, let me know! If you really need some feature bad, let me know! The amount of work I put into this project and number of features I add is directly proportional to how many people ask for them.

Lastly, a big "Thank You!" to [SharpKML](http://sharpkml.codeplex.com/) without which this project would be a lot more work and [VectorLady](http://vectorlady.com/) who authored the icon, which is released under the Creative Commons Lisence 3.0 Attributoin.

KML2SQL is licensed under the [BSD 2 Clause License](https://github.com/Pharylon/KML2SQL/blob/master/License.txt). 