# RoomTemp

[![Build status](https://ci.appveyor.com/api/projects/status/bei1ne3g337xrve4/branch/master?svg=true)](https://ci.appveyor.com/project/bernarden/roomtemp/branch/master)

## Overview
This project contains 3 components:
1. IoT solution. Built using Python to measure, temporarily store and upload temperature data.
2. RESTful API. Written in .Net Core 2.2 to manage temperature readings and related data. Currently supports SQL Server and SQLite databases.
3. React client app. Hosted through the same backend API server. It provides a simple overview of the collected temperature data.
