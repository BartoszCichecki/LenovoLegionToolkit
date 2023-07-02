/* ------------------------------------------------------------
 * SWIG library containing argc and argv multi-argument typemaps

   Use it as follows:

     %apply (int ARGC, char **ARGV) { (size_t argc, const char **argv) }
     extern int mainApp(size_t argc, const char **argv);

   then from lua:

     args = { "arg0", "arg1" }
     mainApp(args)

 * ------------------------------------------------------------ */

%{
SWIGINTERN int SWIG_argv_size(lua_State* L, int index) {
  int n=0;
  while(1){
    lua_rawgeti(L,index,n+1);
    if (lua_isnil(L,-1))
      break;
    ++n;
    lua_pop(L,1);
  }
  lua_pop(L,1);
  return n;
}
%}

%typemap(in) (int ARGC, char **ARGV) {
  if (lua_istable(L,$input)) {
    int i, size = SWIG_argv_size(L,$input);
    $1 = ($1_ltype) size;
    $2 = (char **) malloc((size+1)*sizeof(char *));
    for (i = 0; i < size; i++) {
      lua_rawgeti(L,$input,i+1);
      if (lua_isnil(L,-1))
   break;
      $2[i] = (char *)lua_tostring(L, -1);
      lua_pop(L,1);
    }
    $2[i]=NULL;
  } else {
    $1 = 0; $2 = 0;
    lua_pushstring(L,"Expecting argv array");
    lua_error(L);
  }
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_STRING_ARRAY) (int ARGC, char **ARGV) {
  $1 = lua_istable(L,$input);
}

%typemap(freearg) (int ARGC, char **ARGV) {
  free((char *) $2);
}
