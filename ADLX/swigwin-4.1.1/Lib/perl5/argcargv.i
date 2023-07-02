/* ------------------------------------------------------------
 * SWIG library containing argc and argv multi-argument typemaps
 * ------------------------------------------------------------ */

%typemap(in) (int ARGC, char **ARGV) {
  int i;
  I32 len;
  AV *av = (AV *)SvRV($input);
  if (SvTYPE(av) != SVt_PVAV) {
    SWIG_croak("in method '$symname', Expecting reference to argv array");
    goto fail;
  }
  len = av_len(av) + 1;
  $1 = ($1_ltype) len;
  $2 = (char **) malloc((len+1)*sizeof(char *));
  for (i = 0; i < len; i++) {
    SV **tv = av_fetch(av, i, 0);
    $2[i] = SvPV_nolen(*tv);
  }
  $2[i] = NULL;
}

%typemap(typecheck, precedence=SWIG_TYPECHECK_STRING_ARRAY) (int ARGC, char **ARGV) {
  AV *av = (AV *)SvRV($input);
  $1 = SvTYPE(av) == SVt_PVAV;
}

%typemap(freearg) (int ARGC, char **ARGV) {
  if ($2 != NULL) {
    free((void *)$2);
  }
}
