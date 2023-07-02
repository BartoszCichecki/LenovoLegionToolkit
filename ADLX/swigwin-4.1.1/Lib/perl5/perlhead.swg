#ifdef __cplusplus
/* Needed on some windows machines---since MS plays funny games with the header files under C++ */
#include <math.h>
#include <stdlib.h>
extern "C" {
#endif

#if __GNUC__ >= 10
#if defined(__cplusplus)
#pragma GCC diagnostic push
#pragma GCC diagnostic ignored "-Wvolatile"
#endif
#endif

#include "EXTERN.h"
#include "perl.h"
#include "XSUB.h"

#if __GNUC__ >= 10
#if defined(__cplusplus)
#pragma GCC diagnostic pop
#endif
#endif

/* PERL_REVISION was added in Perl 5.6. */
#if !defined PERL_REVISION || (PERL_REVISION-0 == 5 && PERL_VERSION-0 < 8)
# error SWIG requires Perl >= 5.8.0
#endif

#if defined(WIN32) && defined(PERL_OBJECT) && !defined(PerlIO_exportFILE)
#define PerlIO_exportFILE(fh,fl) (FILE*)(fh)
#endif

#ifndef SvIOK_UV
# define SvIOK_UV(sv)       (SvIOK(sv) && (SvUVX(sv) == SvIVX(sv)))
#endif

#ifndef SvUOK
# define SvUOK(sv)           SvIOK_UV(sv)
#endif

#ifndef IVSIZE
#  ifdef LONGSIZE
#    define IVSIZE LONGSIZE
#  else
#    define IVSIZE 4 /* A bold guess, but the best we can make. */
#  endif
#endif

#ifndef INT2PTR
#  if (IVSIZE == PTRSIZE) && (UVSIZE == PTRSIZE)
#    define PTRV                  UV
#    define INT2PTR(any,d)        (any)(d)
#  else
#    if PTRSIZE == LONGSIZE
#      define PTRV                unsigned long
#    else
#      define PTRV                unsigned
#    endif
#    define INT2PTR(any,d)        (any)(PTRV)(d)
#  endif

#  define NUM2PTR(any,d)  (any)(PTRV)(d)
#  define PTR2IV(p)       INT2PTR(IV,p)
#  define PTR2UV(p)       INT2PTR(UV,p)
#  define PTR2NV(p)       NUM2PTR(NV,p)

#  if PTRSIZE == LONGSIZE
#    define PTR2ul(p)     (unsigned long)(p)
#  else
#    define PTR2ul(p)     INT2PTR(unsigned long,p)
#  endif
#endif /* !INT2PTR */

#ifndef SvPV_nolen
# define SvPV_nolen(x) SvPV(x,PL_na)
#endif

#ifndef get_sv
#  define get_sv perl_get_sv
#endif

#ifndef ERRSV
#  define ERRSV get_sv("@",FALSE)
#endif

#ifndef pTHX_
#define pTHX_
#endif   

#include <string.h>
#ifdef __cplusplus
}
#endif
