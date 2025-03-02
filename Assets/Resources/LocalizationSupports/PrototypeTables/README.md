# Prototype Tables

Prototype Tables contain the localization content for the game. All of tables are in the CSV format.

## Table Structure
| Key | (Language #1) | (Language #2) | ... |
| --- | --- | --- | --- |

The "Key" column is required to identify localization content using the unique identifier.  

The other columns are the localization content for each language. The name of header columns are must follow the Unity's localization system's locale identifier. (e.g. `English(en)`, `Korean(ko)`)

## Difference between Prototype Tables and Unity's table format

In the Unity Engine, there is a column named `Id`. This `Id` data are auto-generated when import the CSV file.

Because of this, the prototype tables must not contain the `Id` column.
