;(function( window, undefined ) {
	MDSQL = function(Basename){
		return FookeDatabase.Open(Basename);
	};
	
	var FookeDatabase = {
		DatabaseName:"",
		thisOptions:{},
		Open:function(Basename)
		{
			this.DatabaseName = Basename;
			this.thisOptions["sss"]="1";
			
			return this;	
		},
		table:function(){
			alert(this.thisOptions["sss"]);
			alert(this.DatabaseName);
			return {
				Delete:function(params){},
				Insert:function(options){},
				Select:function(top,columns,Params){},
				Update:function(){}
			};	
		}
	}
	
})(window);

