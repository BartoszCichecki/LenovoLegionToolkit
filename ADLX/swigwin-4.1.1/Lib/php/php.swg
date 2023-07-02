/* -----------------------------------------------------------------------------
 * php.swg
 *
 * PHP configuration file
 * ----------------------------------------------------------------------------- */

// Default to generating PHP type declarations (for PHP >= 8) except for
// cases which are liable to cause compatibility issues with existing
// bindings.
%feature("php:type", "compat");

%runtime "swigrun.swg"  // Common C API type-checking code
%runtime "swigerrors.swg"    // SWIG errors
%runtime "phprun.swg"	// PHP runtime functions

%include <phpinit.swg> // PHP initialization routine.

%include <const.i>

// use %init %{ "/*code goes here*/ " %}
// or  %minit %{ "/* code goes here*/ " %} to
// insert code in the PHP_MINIT_FUNCTION
#define %minit %insert("init")

// use %rinit %{ "/* code goes here*/ " %} to
// insert code in the PHP_RINIT_FUNCTION
#define %rinit %insert("rinit")

// use %shutdown %{ " /*code goes here*/ " %} to
// insert code in the PHP_MSHUTDOWN_FUNCTION
#define %shutdown  %insert("shutdown")
#define %mshutdown  %insert("shutdown")

// use %rshutdown %{ " /*code goes here*/" %} to
// insert code in the PHP_RSHUTDOWN_FUNCTION
#define %rshutdown  %insert("rshutdown")

/* Typemaps for input parameters by value */

%include <utils.i>

%pass_by_val(bool, "bool", CONVERT_BOOL_IN);

%pass_by_val(size_t, "int", CONVERT_INT_IN);

%pass_by_val(enum SWIGTYPE, "int", CONVERT_INT_IN);

%pass_by_val(signed int, "int", CONVERT_INT_IN);
%pass_by_val(int,"int", CONVERT_INT_IN);
%pass_by_val(unsigned int,"int", CONVERT_INT_IN);

%pass_by_val(signed short, "int", CONVERT_INT_IN);
%pass_by_val(short,"int", CONVERT_INT_IN);
%pass_by_val(unsigned short, "int", CONVERT_INT_IN);

%pass_by_val(signed long, "int", CONVERT_INT_IN);
%pass_by_val(long, "int", CONVERT_INT_IN);
%pass_by_val(unsigned long, "int", CONVERT_INT_IN);

%pass_by_val(signed long long, "int|string", CONVERT_LONG_LONG_IN);
%pass_by_val(long long, "int|string", CONVERT_LONG_LONG_IN);
%pass_by_val(unsigned long long, "int|string", CONVERT_UNSIGNED_LONG_LONG_IN);

%pass_by_val(signed char, "int", CONVERT_INT_IN);
%pass_by_val(char, "string", CONVERT_CHAR_IN);
%pass_by_val(unsigned char, "int", CONVERT_INT_IN);

%pass_by_val(float, "float", CONVERT_FLOAT_IN);

%pass_by_val(double, "float", CONVERT_FLOAT_IN);

%pass_by_val(char *, "string", CONVERT_STRING_IN);
%typemap(in) char *& = const char *&;
%typemap(directorout) char *& = const char *&;

// char array can be in/out, though the passed string may not be big enough...
// so we have to size it
%typemap(in, phptype="string") char[ANY]
%{
   convert_to_string(&$input);
   $1 = ($1_ltype) Z_STRVAL($input);
%}

%typemap(in, phptype="string") (char *STRING, int LENGTH), (char *STRING, size_t LENGTH) %{
   convert_to_string(&$input);
   $1 = ($1_ltype) Z_STRVAL($input);
   $2 = ($2_ltype) Z_STRLEN($input);
%}

/* Object passed by value. Convert to a pointer */
%typemap(in, phptype="SWIGTYPE") SWIGTYPE ($&1_ltype tmp)
%{
  if (SWIG_ConvertPtr(&$input, (void **) &tmp, $&1_descriptor, 0) < 0 || tmp == NULL) {
    zend_type_error("Expected $&1_descriptor for argument $argnum of $symname");
    return;
  }
  $1 = *tmp;
%}

%typemap(directorout) SWIGTYPE ($&1_ltype tmp)
%{
  if (SWIG_ConvertPtr($input, (void **) &tmp, $&1_descriptor, 0) < 0 || tmp == NULL) {
    zend_type_error("Expected $&1_descriptor for argument $argnum of $symname");
    SWIG_fail;
  }
  $result = *tmp;
%}

%typemap(in, phptype="?SWIGTYPE") SWIGTYPE *,
	     SWIGTYPE []
%{
  if (SWIG_ConvertPtr(&$input, (void **) &$1, $1_descriptor, 0) < 0) {
    zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
    return;
  }
%}

%typemap(directorout) SWIGTYPE * (swig_owntype own),
		      SWIGTYPE [] (swig_owntype own)
%{
  if (SWIG_ConvertPtrAndOwn($input, (void **)&$result, $1_descriptor, SWIG_POINTER_DISOWN, &own) < 0) {
    zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
    SWIG_fail;
  }
  swig_acquire_ownership_obj((void*)$result, own);
%}

%typemap(in, phptype="SWIGTYPE") SWIGTYPE &
%{
  if (SWIG_ConvertPtr(&$input, (void **) &$1, $1_descriptor, 0) < 0 || $1 == NULL) {
    zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
    return;
  }
%}
%typemap(in, fragment="<memory>") SWIGTYPE && (void *argp = 0, int res = 0, std::unique_ptr<$*1_ltype> rvrdeleter) %{
  res = SWIG_ConvertPtr(&$input, &argp, $descriptor, SWIG_POINTER_RELEASE);
  if (!SWIG_IsOK(res)) {
    if (res == SWIG_ERROR_RELEASE_NOT_OWNED) {
      zend_type_error("Cannot release ownership as memory is not owned for argument $argnum of $1_descriptor of $symname");
      return;
    } else {
      zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
      return;
    }
  }
  if (!argp) {
    zend_type_error("Invalid null reference for argument $argnum of $1_descriptor of $symname");
    return;
  }
  $1 = ($1_ltype)argp;
  rvrdeleter.reset($1);
%}

%typemap(directorout) SWIGTYPE & ($1_ltype tmp),
		      SWIGTYPE && ($1_ltype tmp)
%{
  if (SWIG_ConvertPtr($input, (void **) &tmp, $1_descriptor, 0) < 0 || tmp == NULL) {
    zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
    SWIG_fail;
  }
  $result = tmp;
%}

%typemap(in, phptype="?SWIGTYPE") SWIGTYPE *const& ($*ltype temp)
%{
  if (SWIG_ConvertPtr(&$input, (void **) &temp, $*1_descriptor, 0) < 0) {
    zend_type_error("Expected $*1_descriptor for argument $argnum of $symname");
    return;
  }
  $1 = ($1_ltype)&temp;
%}

%typemap(in, phptype="?SWIGTYPE") SWIGTYPE *DISOWN
%{
  if (SWIG_ConvertPtr(&$input, (void **) &$1, $1_descriptor, SWIG_POINTER_DISOWN) < 0) {
    zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
    return;
  }
%}

%typemap(argout) SWIGTYPE *,
                 SWIGTYPE [],
                 SWIGTYPE &,
                 SWIGTYPE &&;

%typemap(in, phptype="?SWIGTYPE") void *
%{
  if (SWIG_ConvertPtr(&$input, (void **) &$1, 0, 0) < 0) {
    /* Allow NULL from php for void* */
    if (Z_ISNULL($input)) {
      $1=0;
    } else {
      zend_type_error("Expected $1_descriptor for argument $argnum of $symname");
      return;
    }
  }
%}

/* Special case when void* is passed by reference so it can be made to point
   to opaque api structs */
%typemap(in, phptype="?SWIG\\_p_void", byref=1) void ** ($*1_ltype ptr, int force),
             void *& ($*1_ltype ptr, int force)
{
  /* If they pass NULL by reference, make it into a void*
     This bit should go in arginit if arginit support init-ing scripting args */
  if (SWIG_ConvertPtr(&$input, (void **) &$1, $1_descriptor, 0) < 0) {
    /* So... we didn't get a ref or ptr, but we'll accept NULL by reference */
    if (!(Z_ISREF($input) && Z_ISNULL_P(Z_REFVAL($input)))) {
      /* wasn't a pre/ref/thing, OR anything like an int thing */
      zend_throw_exception(zend_ce_type_error, "Type error in argument $arg of $symname", 0);
      goto fail;
    }
  }
  force=0;
  if (arg1==NULL) {
#ifdef __cplusplus
    ptr=new $*1_ltype();
#else
    ptr=($*1_ltype) calloc(1,sizeof($*1_ltype));
#endif
    $1=&ptr;
    /* have to passback arg$arg too */
    force=1;
  }
}
%typemap(argout) void **,
                 void *&
%{
  if (force$argnum && Z_ISREF($input)) {
    SWIG_SetPointerZval(Z_REFVAL($input), (void*) ptr$argnum, $*1_descriptor, 1);
  }
%}

/* Typemap for output values */

%typemap(out, phptype="int")
              int,
              unsigned int,
              short,
              unsigned short,
              long,
              unsigned long,
              signed char,
              unsigned char,
              size_t
%{
  RETVAL_LONG($1);
%}

%typemap(out, phptype="int") enum SWIGTYPE
%{
  RETVAL_LONG((long)$1);
%}

%typemap(out, phptype="int|string") long long
%{
  if ((long long)LONG_MIN <= $1 && $1 <= (long long)LONG_MAX) {
    RETVAL_LONG((long)($1));
  } else {
    RETVAL_NEW_STR(zend_strpprintf(0, "%lld", (long long)$1));
  }
%}
%typemap(out, phptype="int|string") unsigned long long
%{
  if ($1 <= (unsigned long long)LONG_MAX) {
    RETVAL_LONG((long)($1));
  } else {
    RETVAL_NEW_STR(zend_strpprintf(0, "%llu", (unsigned long long)$1));
  }
%}

%typemap(out, phptype="int")
              const int &,
              const unsigned int &,
              const short &,
              const unsigned short &,
              const long &,
              const unsigned long &,
              const signed char &,
              const unsigned char &,
              const bool &,
              const size_t &
%{
  RETVAL_LONG(*$1);
%}

%typemap(out, phptype="int") const enum SWIGTYPE &
%{
  RETVAL_LONG((long)*$1);
%}

%typemap(out, phptype="int") const enum SWIGTYPE &&
%{
  RETVAL_LONG((long)*$1);
%}

%typemap(out, phptype="int|string") const long long &
%{
  if ((long long)LONG_MIN <= *$1 && *$1 <= (long long)LONG_MAX) {
    RETVAL_LONG((long)(*$1));
  } else {
    RETVAL_NEW_STR(zend_strpprintf(0, "%lld", (long long)(*$1)));
  }
%}
%typemap(out, phptype="int|string") const unsigned long long &
%{
  if (*$1 <= (unsigned long long)LONG_MAX) {
    RETVAL_LONG((long)(*$1));
  } else {
    RETVAL_NEW_STR(zend_strpprintf(0, "%llu", (unsigned long long)(*$1)));
  }
%}

%typemap(directorin) int,
              unsigned int,
              short,
              unsigned short,
              long,
              unsigned long,
              signed char,
              unsigned char,
              size_t,
              enum SWIGTYPE
%{
  ZVAL_LONG($input,$1);
%}

%typemap(directorin) enum SWIGTYPE
%{
  ZVAL_LONG($input, (long)$1_name);
%}

%typemap(directorin) char *, char []
%{
    if(!$1) {
      ZVAL_NULL($input);
    } else {
      ZVAL_STRING($input, (const char*)$1);
    }
%}

%typemap(out, phptype="bool") bool
%{
  RETVAL_BOOL(($1) ? 1 : 0);
%}

%typemap(out, phptype="bool") const bool &
%{
  RETVAL_BOOL((*$1) ? 1 : 0);
%}

%typemap(directorin) bool
%{
  ZVAL_BOOL($input, ($1) ? 1 : 0);
%}

%typemap(out, phptype="float") float,
              double
%{
  RETVAL_DOUBLE($1);
%}

%typemap(out, phptype="float") const float &,
              const double &
%{
  RETVAL_DOUBLE(*$1);
%}

%typemap(directorin) float,
                     double
%{
  ZVAL_DOUBLE($input, $1);
%}

%typemap(out, phptype="string") char
%{
  RETVAL_STRINGL(&$1, 1);
%}

%typemap(out, phptype="string") const char &
%{
  RETVAL_STRINGL(&*$1, 1);
%}

%typemap(out, phptype="string") char []
%{
  RETVAL_STRING((const char *)$1);
%}

%typemap(out, phptype="?string") char *
%{
  if (!$1) {
    RETVAL_NULL();
  } else {
    RETVAL_STRING((const char *)$1);
  }
%}

%typemap(out, phptype="?string") char *&
%{
  if (!*$1) {
    RETVAL_NULL();
  } else {
    RETVAL_STRING((const char *)*$1);
  }
%}

%typemap(out, phptype="?SWIGTYPE") SWIGTYPE *
%{
  SWIG_SetPointerZval($result, (void *)$1, $1_descriptor, $owner);
%}

%typemap(out, phptype="SWIGTYPE")
              SWIGTYPE [],
              SWIGTYPE &,
              SWIGTYPE &&
%{
  SWIG_SetPointerZval($result, (void *)$1, $1_descriptor, $owner);
%}

%typemap(out, phptype="?SWIGTYPE") SWIGTYPE *const&
%{
  SWIG_SetPointerZval($result, (void *)*$1, $*1_descriptor, $owner);
%}

%typemap(directorin) SWIGTYPE *,
                     SWIGTYPE [],
                     SWIGTYPE &,
                     SWIGTYPE &&
%{
  ZVAL_UNDEF($input);
  SWIG_SetPointerZval($input, (void *)&$1, $1_descriptor, $owner);
%}

%typemap(out, phptype="SWIGTYPE") SWIGTYPE (CLASS::*)
{
  void * p = emalloc(sizeof($1));
  memcpy(p, &$1, sizeof($1));
  SWIG_SetPointerZval($result, (void *)p, $&1_descriptor, 1);
}

%typemap(in, phptype="SWIGTYPE") SWIGTYPE (CLASS::*)
{
  void * p = SWIG_Z_FETCH_OBJ_P(&$input)->ptr;
  memcpy(&$1, p, sizeof($1));
}

%typemap(out, phptype="?SWIGTYPE") SWIGTYPE *DYNAMIC
{
  swig_type_info *ty = SWIG_TypeDynamicCast($1_descriptor, (void **) &$1);
  SWIG_SetPointerZval($result, (void *)$1, ty, $owner);
}

%typemap(out, phptype="SWIGTYPE") SWIGTYPE &DYNAMIC
{
  swig_type_info *ty = SWIG_TypeDynamicCast($1_descriptor, (void **) &$1);
  SWIG_SetPointerZval($result, (void *)$1, ty, $owner);
}

%typemap(out, phptype="SWIGTYPE") SWIGTYPE
{
#ifdef __cplusplus
  $&1_ltype resultobj = new $1_ltype($1);
#else
  $&1_ltype resultobj = ($&1_ltype) malloc(sizeof($1_type));
  memcpy(resultobj, &$1, sizeof($1_type));
#endif
  SWIG_SetPointerZval($result, (void *)resultobj, $&1_descriptor, 1);
}

%typemap(directorin) SWIGTYPE
%{
  ZVAL_UNDEF($input);
  SWIG_SetPointerZval($input, (new $1_ltype(SWIG_STD_MOVE($1))), $&1_descriptor, 1);
%}

%typemap(out, phptype="void") void ""

%typemap(out, phptype="string") char [ANY]
{
  size_t len = 0;
  while (len < $1_dim0 && $1[len]) ++len;
  RETVAL_STRINGL($1, len);
}

// This typecheck does hard checking for proper argument type.  If you want
// an argument to be converted from a different PHP type, you must convert
// it yourself before passing it (e.g. (string)4.7 or (int)"6").
%define %php_typecheck(_type,_prec,is)
%typemap(typecheck,precedence=_prec) _type, const _type &
 " $1 = (Z_TYPE($input) == is);"
%enddef

// Like %php_typecheck but allows either of two values.
%define %php_typecheck2(_type,_prec,is1,is2)
%typemap(typecheck,precedence=_prec) _type, const _type &
 " $1 = (Z_TYPE($input) == is1 || Z_TYPE($input) == is2);"
%enddef

%php_typecheck(int,SWIG_TYPECHECK_INTEGER,IS_LONG)
%php_typecheck(unsigned int,SWIG_TYPECHECK_UINT32,IS_LONG)
%php_typecheck(short,SWIG_TYPECHECK_INT16,IS_LONG)
%php_typecheck(unsigned short,SWIG_TYPECHECK_UINT16,IS_LONG)
%php_typecheck(long,SWIG_TYPECHECK_INT32,IS_LONG)
%php_typecheck(unsigned long,SWIG_TYPECHECK_UINT32,IS_LONG)
%php_typecheck(long long,SWIG_TYPECHECK_INT64,IS_LONG)
%php_typecheck(unsigned long long,SWIG_TYPECHECK_UINT64,IS_LONG)
%php_typecheck(signed char,SWIG_TYPECHECK_INT8,IS_LONG)
%php_typecheck(unsigned char,SWIG_TYPECHECK_UINT8,IS_LONG)
%php_typecheck(size_t,SWIG_TYPECHECK_SIZE,IS_LONG)
%php_typecheck(enum SWIGTYPE,SWIG_TYPECHECK_INTEGER,IS_LONG)
%php_typecheck2(bool,SWIG_TYPECHECK_BOOL,IS_TRUE,IS_FALSE)
%php_typecheck(float,SWIG_TYPECHECK_FLOAT,IS_DOUBLE)
%php_typecheck(double,SWIG_TYPECHECK_DOUBLE,IS_DOUBLE)
%php_typecheck(char,SWIG_TYPECHECK_CHAR,IS_STRING)

%typemap(typecheck,precedence=SWIG_TYPECHECK_STRING) char *, char *&
 " $1 = (Z_TYPE($input) == IS_STRING || Z_TYPE($input) == IS_NULL); "

%typemap(typecheck,precedence=SWIG_TYPECHECK_STRING) char []
 " $1 = (Z_TYPE($input) == IS_STRING); "

%typecheck(SWIG_TYPECHECK_POINTER) SWIGTYPE
{
  void *tmp;
  $1 = (SWIG_ConvertPtr(&$input, (void **)&tmp, $&1_descriptor, SWIG_POINTER_NO_NULL) >= 0);
}

%typecheck(SWIG_TYPECHECK_POINTER)
             SWIGTYPE *,
             SWIGTYPE [],
             SWIGTYPE *const&
{
  void *tmp;
  $1 = (SWIG_ConvertPtr(&$input, (void**)&tmp, $1_descriptor, 0) >= 0);
}

%typecheck(SWIG_TYPECHECK_POINTER)
             SWIGTYPE &,
             SWIGTYPE &&
{
  void *tmp;
  $1 = (SWIG_ConvertPtr(&$input, (void**)&tmp, $1_descriptor, SWIG_POINTER_NO_NULL) >= 0);
}

%typecheck(SWIG_TYPECHECK_POINTER) SWIGTYPE *const&
{
  void *tmp;
  $1 = (SWIG_ConvertPtr(&$input, (void**)&tmp, $*1_descriptor, 0) >= 0);
}

%typecheck(SWIG_TYPECHECK_VOIDPTR) void *
{
  void *tmp;
  $1 = (SWIG_ConvertPtr(&$input, (void**)&tmp, 0, 0) >= 0);
}

/* Exception handling */

%typemap(throws) int,
                 long,
                 short,
                 unsigned int,
                 unsigned long,
                 unsigned short %{
  zend_throw_exception(NULL, "C++ $1_type exception thrown", $1);
  goto fail;
%}

%typemap(throws) SWIGTYPE, SWIGTYPE &, SWIGTYPE &&, SWIGTYPE *, SWIGTYPE [], SWIGTYPE [ANY] %{
  (void)$1;
  zend_throw_exception(NULL, "C++ $1_type exception thrown", 0);
  goto fail;
%}

%typemap(throws) char * %{
  zend_throw_exception(NULL, $1, 0);
  goto fail;
%}

/* Array reference typemaps */
%apply SWIGTYPE & { SWIGTYPE ((&)[ANY]) }
%apply SWIGTYPE && { SWIGTYPE ((&&)[ANY]) }

/* const pointers */
%apply SWIGTYPE * { SWIGTYPE *const }
%apply SWIGTYPE (CLASS::*) { SWIGTYPE (CLASS::*const) }
%apply SWIGTYPE & { SWIGTYPE (CLASS::*const&) }

/* php keywords */
%include <phpkw.swg>

/* PHP known interfaces */
%include <phpinterfaces.i>
