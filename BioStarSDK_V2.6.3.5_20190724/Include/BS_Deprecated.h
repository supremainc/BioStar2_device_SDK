/*
 * BS_Deprecated.h
 *
 *  Created on: 2018. 2. 13.
 *      Author: charlie
 */

#ifndef CORE_SRC_BS_DEPRECATED_H_
#define CORE_SRC_BS_DEPRECATED_H_


// Deprecate class, structure, function
#ifdef __GNUC__
#define DEPRECATED_CLASS __attribute__((deprecated))
#elif defined(_MSC_VER)
#define DEPRECATED_CLASS __declspec(deprecated)
#else
#define DEPRECATED_CLASS
#pragma message("DEPRECATED is not defined for this compiler")
#endif

#define DEPRECATED_STRUCT		DEPRECATED_CLASS
#define DEPRECATED_FUNC			DEPRECATED_CLASS


// Deprecated enum, macro item
#ifdef __GNUC__
typedef __attribute__((deprecated))int DEPRECATED_ENUM;
#elif defined(_MSC_VER)
typedef __declspec(deprecated)int DEPRECATED_ENUM;
#endif

#define DEPRECATED_MAC			DEPRECATED_ENUM


// Deprecated const, typedef
#ifdef __GNUC__
#define DEPRECATED_ITEM(x)	__attribute__((deprecated)) x
#elif defined(_MSC_VER)
#define DEPRECATED_ITEM(x)	__declspec(deprecated) x
#endif

#define DEPRECATED_TYPEDEF		DEPRECATED_ITEM
#define DEPRECATED_CONST        DEPRECATED_ITEM


#ifdef TEST_CODE
enum MyEnum
{
	EN_VAL_1 = 1,
	EN_VAL_2 = 2,
};

#ifdef __GNUC__
typedef __attribute__((deprecated))MyEnum MYENUM_DEPRECATED;
#elif defined(_MSC_VER)
typedef __declspec(deprecated)MyEnum MYENUM_DEPRECATED;
#endif
#define EN_VAL_2        ((MYENUM_DEPRECATED) EN_VAL_2)

//#pragma deprecated(EN_VAL_2)


DEPRECATED_FUNC int addFunc(int a, int b)
{
	return a + b;
}

class DEPRECATED_CLASS ClassA
{
public:
	ClassA() {}
	~ClassA() {}
	int add(int a, int b) {return a + b;}
};

class ClassB
{
public:
	ClassB() {}
	~ClassB() {}
	DEPRECATED_FUNC int multi(int a, int b) {return a * b;}
	int div(int a, int b) {return a / b;}
};

#define MACRO_1				(DEPRECATED_MAC)-1


int main()
{
	printf("Value:%d\n", addFunc(1, 2));
	ClassA a;
	ClassB b;
	printf("Value:%d\n", a.add(1, 2));
	printf("Value:%d\n", b.multi(1, 2));

	printf("Value:%d\n", EN_VAL_1);
	printf("Value:%d\n", EN_VAL_2);

	int mac0 = MACRO_1;

	return 0;
}
#endif


#endif /* CORE_SRC_BS_DEPRECATED_H_ */
