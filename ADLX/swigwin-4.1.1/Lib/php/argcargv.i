/* ------------------------------------------------------------
 * SWIG library containing argc and argv multi-argument typemaps
 * ------------------------------------------------------------ */

%typemap(in) (int ARGC, char **ARGV) {
  int len, i;
  zval *val;
  zend_array *ar;
  if (Z_TYPE($input) != IS_ARRAY) {
    SWIG_PHP_Error(E_ERROR, "Type error in '$symname'. Expected array");
    goto fail;
  }
  ar = Z_ARR($input);
  len = zend_array_count(ar);
  $1 = ($1_ltype) len;
  $2 = (char **) malloc((len+1)*sizeof(char *));
  i = 0;
  ZEND_HASH_FOREACH_VAL(ar, val) {
    if (Z_TYPE(*val) != IS_STRING) {
      SWIG_PHP_Error(E_ERROR, "Array must use strings only, in '$symname'.");
      goto fail;
    }
    if (i == len) {
      SWIG_PHP_Error(E_ERROR, "Array is bigger than zend report in '$symname'.");
      goto fail;
    }
    $2[i++] = Z_STRVAL(*val);
  } ZEND_HASH_FOREACH_END();
  $2[i] = NULL;
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_STRING_ARRAY) (int ARGC, char **ARGV) {
  $1 = Z_TYPE($input) == IS_ARRAY;
}

%typemap(freearg) (int ARGC, char **ARGV) {
  if ($2 != NULL) {
    free((void *)$2);
  }
}
