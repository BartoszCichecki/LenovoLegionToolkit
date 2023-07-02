%define %pass_by_ref( TYPE, PHP_TYPE, CONVERT_IN, CONVERT_OUT )
%typemap(in,byref=1,phptype=PHP_TYPE) TYPE *REF ($*1_ltype tmp),
             TYPE &REF ($*1_ltype tmp)
%{
  if (Z_ISREF($input)) {
    CONVERT_IN(tmp, $*1_ltype, $input);
    $1 = &tmp;
  } else {
    zend_type_error(SWIG_PHP_Arg_Error_Msg($argnum, Expected a reference));
  }
%}
%typemap(argout) TYPE *REF,
                 TYPE &REF
%{
  if (Z_ISREF($input)) {
    CONVERT_OUT(Z_REFVAL($input), tmp$argnum);
  }
%}
%enddef

%pass_by_ref( size_t, "int", CONVERT_INT_IN, ZVAL_LONG );

%pass_by_ref( signed int, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( int, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( unsigned int, "int", CONVERT_INT_IN, ZVAL_LONG );

%pass_by_ref( signed short, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( short, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( unsigned short, "int", CONVERT_INT_IN, ZVAL_LONG );

%pass_by_ref( signed long, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( long, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( unsigned long, "int", CONVERT_INT_IN, ZVAL_LONG );

%pass_by_ref( signed char, "int", CONVERT_INT_IN, ZVAL_LONG );
%pass_by_ref( char, "string", CONVERT_CHAR_IN, ZVAL_STRING );
%pass_by_ref( unsigned char, "int", CONVERT_INT_IN, ZVAL_LONG );

%pass_by_ref( float, "float", CONVERT_FLOAT_IN, ZVAL_DOUBLE );
%pass_by_ref( double, "float", CONVERT_FLOAT_IN, ZVAL_DOUBLE );

%pass_by_ref( char *, "string", CONVERT_CHAR_IN, ZVAL_STRING );
