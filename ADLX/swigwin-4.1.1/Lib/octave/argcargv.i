/* ------------------------------------------------------------
 * SWIG library containing argc and argv multi-argument typemaps
 * ------------------------------------------------------------ */

%typemap(in) (int ARGC, char **ARGV) {
  if ($input.is_scalar_type()) {
    $1 = 0; $2 = NULL;
    %argument_fail(SWIG_TypeError, "'int ARGC, char **ARGV' is not a list", $symname, $argnum);
  }
  octave_value_list list = $input.list_value();
  int i, len = list.length();
  $1 = ($1_ltype) len;
  $2 = (char **) malloc((len+1)*sizeof(char *));
  for (i = 0; i < len; i++) {
    if (!list(i).is_string()) {
      $1 = 0;
      %argument_fail(SWIG_TypeError, "'int ARGC, char **ARGV' use a non-string", $symname, $argnum);
    }
    $2[i] = (char *)list(i).string_value().c_str();
  }
  $2[i] = NULL;
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_STRING_ARRAY) (int ARGC, char **ARGV) {
  $1 = 0;
  const octave_value& ov = $input;
  if (!ov.is_scalar_type()) {
    octave_value_list list = ov.list_value();
    int i, len = list.length();
    $1 = 1;
    for (i = 0; i < len; i++) {
      if (!list(i).is_string()) {
        $1 = 0;
        break;
      }
    }
  }
}

%typemap(freearg) (int ARGC, char **ARGV) {
  if ($2 != NULL) {
    free((void *)$2);
  }
}
