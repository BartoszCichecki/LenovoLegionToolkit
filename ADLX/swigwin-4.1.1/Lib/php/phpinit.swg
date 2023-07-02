
/* ------------------------------------------------------------
 * The start of the PHP initialization function
 * ------------------------------------------------------------ */

%insert(init) "swiginit.swg"

%init %{
SWIG_php_minit {
  zend_class_entry SWIGUNUSED internal_ce;
  SWIG_InitializeModule((void*)&module_number);
#if PHP_MAJOR_VERSION == 8 && PHP_MINOR_VERSION == 0
  /* This hack is needed to avoid segfaults. */
  EG(class_table) = CG(class_table);
#endif
%}
