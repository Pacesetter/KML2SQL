#KML2SQL v 1.0

###Overview
KML2SQL is a program that takes KML files and uploades them to MSSQL geography or geometry files. I'm now feeling good enough about this that I'm going to call it Version 1.0.

You can install the program from [here](http://goo.gl/TbYhK). 

To-Do List:

* Add support for <SimpleData> and <Timespan> attributes as extra columns in SQL.
* Provide support for other database types. MSSQL is the first because it has great geospatial support, but getting it working with mySQL is a high priority. If there's interest, I may add PostgreSQL and mongoDB support as well.
* Provide support for polygons with lines that intersect (in real life they shouldn't, but KML lets you draw polygons like that. Code would need to be able to detect that and fix, which would be fairly hard).

If you have any questions, feel free to post issues here on GitHub, email me at zach<at>zachshuford<dotcom>. Or hit me up on [Google+](https://plus.google.com/100663438782533486183).