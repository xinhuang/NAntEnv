# NAntEnv

NAntEnv will load all environment variables into NAnt properties. 

It's your best friend when you need to load Jenkins job build parameters into NAnt scripts.

## Usage

All environment variables will be loaded into NAnt properties if it matches NAnt property naming rules. (`\w[\w\-.]+$`) 

By default, underscore will be replaced by dot, and all letter is converted to lowercase.

    <target name="MyTarget">
        <load-environment/>
        
        <echo message="ENV_VAR is loaded into env.var, which is ${env.var}"/>
    </target>
    
## Options

* `delimeters`: Specify one or more delimeters to be replaced. Default is underscore.
* `tolower`: If is true, will convert all property names to lowercase; otherwise left as it is. Default is true.
* `target`: Where to load environment variables. Choices are: `process`(default), `user` or `machine`.
* `verbose`: Print every environment variables that is loaded.

## License

[GNU Lesser General Public License]


  [GNU Lesser General Public License]: http://www.gnu.org/copyleft/lgpl.html
  