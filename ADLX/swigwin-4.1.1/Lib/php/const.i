/* -----------------------------------------------------------------------------
 * const.i
 *
 * Typemaps for constants
 * ----------------------------------------------------------------------------- */
%typemap(classconsttab) int,
                        unsigned int,
                        short,
                        unsigned short,
                        long,
                        unsigned long,
                        unsigned char,
                        signed char,
                        enum SWIGTYPE %{
  zend_declare_class_constant_long(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, ($1_type)($value));
%}

%typemap(classconsttab) bool %{
  zend_declare_class_constant_bool(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, ($1_type)($value));
%}

%typemap(classconsttab) float,
                        double %{
  zend_declare_class_constant_double(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, $value);
%}

%typemap(classconsttab) char %{
{
  char swig_char = $value;
  zend_declare_class_constant_stringl(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, &swig_char, 1);
}
%}

%typemap(classconsttab) char *,
                        const char *,
                        char [],
                        const char [] %{
  zend_declare_class_constant_string(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, $value);
%}

// This creates a zend_object to wrap the pointer, and we can't do that
// before the Zend runtime has been initialised so we delay it until
// RINIT.  The downside is it then happens for every request.
%typemap(classconsttab,rinit=1) SWIGTYPE *,
                        SWIGTYPE &,
                        SWIGTYPE &&,
                        SWIGTYPE [] %{
{
  zval z;
  ZVAL_UNDEF(&z);
  SWIG_SetPointerZval(&z, (void*)($value), $1_descriptor, 0);
  zval_copy_ctor(&z);
  zend_declare_class_constant(SWIG_Php_ce_$class, "$const_name", sizeof("$const_name") - 1, &z);
}
%}

%typemap(classconsttab) SWIGTYPE (CLASS::*) ""

%typemap(consttab) int,
                   unsigned int,
                   short,
                   unsigned short,
                   long,
                   unsigned long,
                   unsigned char,
                   signed char,
                   enum SWIGTYPE
  "SWIG_LONG_CONSTANT($symname, ($1_type)($value));";

%typemap(consttab) bool
  "SWIG_BOOL_CONSTANT($symname, ($1_type)($value));";

%typemap(consttab) float,
                   double
  "SWIG_DOUBLE_CONSTANT($symname, $value);";

%typemap(consttab) char
  "SWIG_CHAR_CONSTANT($symname, $value);";

%typemap(consttab) char *,
                   const char *,
                   char [],
                   const char []
  "SWIG_STRING_CONSTANT($symname, $value);";

// This creates a zend_object to wrap the pointer, and we can't do that
// before the Zend runtime has been initialised so we delay it until
// RINIT.  The downside is it then happens for every request.
%typemap(consttab,rinit=1) SWIGTYPE *,
                   SWIGTYPE &,
                   SWIGTYPE &&,
                   SWIGTYPE [] {
  zend_constant c;
  ZVAL_UNDEF(&c.value);
  SWIG_SetPointerZval(&c.value, (void*)($value), $1_descriptor, 0);
  zval_copy_ctor(&c.value);
  c.name = zend_string_init("$symname", sizeof("$symname") - 1, 0);
  SWIG_ZEND_CONSTANT_SET_FLAGS(&c, CONST_CS, module_number);
  zend_register_constant(&c);
}

/* Handled as a global variable. */
%typemap(consttab) SWIGTYPE (CLASS::*) ""
