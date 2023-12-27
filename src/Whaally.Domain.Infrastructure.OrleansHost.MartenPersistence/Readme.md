# Orleans - Marten integration
This package provides support to run the Whaally.Domain project using Orleans and Marten. Where Orleans is used to provide
elastic scalability, Marten is used for persistence using PostgreSQL. This project provides a default implementation of 
an aggregate handler reading data from and persisting data to a PostgreSQL database through Marten.

## Installation
To start using this package you'll need to do the following things:

- Ensure you have set up your Orleans cluster
- [Configure Marten](https://martendb.io/configuration/hostbuilder.html)

