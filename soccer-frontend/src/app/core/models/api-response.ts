export interface Result<T> {
  isSuccess: boolean;
  data: T;
  errorMessage: string | null;
}

export interface PaginatedResult<T> {
  isSuccess: boolean;
  data: {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
  };
  errorMessage: string | null;
}
