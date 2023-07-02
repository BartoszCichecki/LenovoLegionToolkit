/* -----------------------------------------------------------------------------
 * ccomplex.i
 *
 * C complex typemaps
 * ISO C99:  7.3 Complex arithmetic <complex.h>
 * ----------------------------------------------------------------------------- */


%include <javascriptcomplex.swg>

%{
#include <complex.h>
%}

#define complex _Complex

/* C complex constructor */
#define CCplxConst(r, i) ((r) + I*(i))

%swig_cplxflt_convn(float _Complex, CCplxConst, creal, cimag);
%swig_cplxdbl_convn(double _Complex, CCplxConst, creal, cimag);
%swig_cplxdbl_convn(_Complex, CCplxConst, creal, cimag);

/* declaring the typemaps */
%typemaps_primitive(SWIG_TYPECHECK_CPLXFLT, float _Complex);
%typemaps_primitive(SWIG_TYPECHECK_CPLXDBL, double _Complex);
%typemaps_primitive(SWIG_TYPECHECK_CPLXDBL, _Complex);
