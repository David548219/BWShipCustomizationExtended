# ShipCustomizationExtended
Client side Blackwake mod for extended ship customization over the network.

**How to install**

- Install latest version of da_google's mod loader
- Unzip mod archive into *.../steamapps/common/Blackwake/Blackwake_Data/Managed/Mods/*

**How to use**

Mod customizes ships based on the ship name. When captain constructs the ship he needs to add a customization tag.

**Tag syntax**

Everything after '#' in a ship name is considered a customization tag (name example "Submarine #SrFfLl").

Customization tag consists of operators, some operators require a value to be specified after them.

**Operators**

*Sails color*. Operator char is **S**, operator requires a color value to be specified. Example: Sl. Changes sail color. 

*Rigging color*. Operator char is **R**, operator requires a color value to be specified. Example: Rb. Changes rigging & ropes color.

*Lighting color*. Operator char is **L**, operator requires a color value to be specified. Example: Lr. Changes all light sources color.

*Flag texture*. Operator char is **F**, operator requires a flag texture value to be specified. Example: Ff. Changes flag texture.

**Values**

*Colors*

Black (*b*), Blue (*l*), Cyan (*c*), Gray (*a*), Green (*g*), Magenta (*m*), Red (*r*), White (*w*), Yellow (*y*), Orange (*o*).

*Flag textures*

Frog (*f*), Pirate frog (*p*), USSR flag (*u*).

**Mod limitations**

Customization is not reset to default look after ship changes name. Using customization limits name length, because you have to provide tags.